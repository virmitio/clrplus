﻿//-----------------------------------------------------------------------
// <copyright company="CoApp Project">
//     Copyright (c) 2010-2013 Garrett Serack and CoApp Contributors. 
//     Contributors can be discovered using the 'git log' command.
//     All rights reserved.
// </copyright>
// <license>
//     The software is licensed under the Apache 2.0 License (the "License")
//     You may not use the software except in compliance with the License. 
// </license>
//-----------------------------------------------------------------------

namespace ClrPlus.Scripting.Languages.PropertySheetV3 {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Core.Collections;
    using Core.Exceptions;
    using Core.Extensions;
    using Mapping;
    using RValue;


    public class ObjectNode : XDictionary<Selector, INode>, INode, IValueContext {
        internal readonly Lazy<IDictionary<string, string>> Aliases = new Lazy<IDictionary<string, string>>(() => new XDictionary<string, string>());
        private readonly Lazy<IDictionary<string, IValue>> _metadata = new Lazy<IDictionary<string, IValue>>(() => new XDictionary<string, IValue>());
        
        public IDictionary<Selector, ObjectNode> Children;
        public IDictionary<Selector, PropertyNode> Properties;
        
        protected ObjectNode() {
            Properties = new DelegateDictionary<Selector, PropertyNode>(
                clear: Clear,
                remove: Remove,
                keys: () => Keys.Where(each => this[each] is PropertyNode).ToList(),
                set: (index, value) => this.AddOrSet(index, value),
                get: index => {
                    if (ContainsKey(index)) {
                        var v = this[index] as PropertyNode;
                        if (v == null) {
                            throw new ClrPlusException("Index {0} is not a PropertyNode, but an Object".format(index));
                        }
                        return v;
                    }
                    var r = new PropertyNode();
                    this.AddOrSet(index, r);
                    return r;
                });

            Children = new DelegateDictionary<Selector, ObjectNode>(
                clear: Clear,
                remove: Remove,
                keys: () => Keys.Where(each => this[each] is ObjectNode).ToList(),
                set: (index, value) => this.AddOrSet(index, value),
                get: index => {
                    if (ContainsKey(index)) {
                        var v = this[index] as ObjectNode;
                        if (v == null) {
                            throw new ClrPlusException("Index {0} is not a Object, but a PropertyNode".format(index));
                        }
                        return v;
                    }
                    var r = new ObjectNode(this, index);
                    this.AddOrSet(index, r);
                    return r;
                });
        }

        protected ObjectNode(PropertySheet root) : this() {
            // this is for imported sheets that share the same root.
            Parent = root;
            Selector = Selector.Empty;
        }

        internal ObjectNode(ObjectNode parent, Selector selector) : this() {
            Parent = parent;
            Selector = selector;
        }

        public Selector Selector {get; private set;}
        public virtual PropertySheet Root { get {
            return Parent == null ? this as PropertySheet : Parent.Root;
        }}
        public ObjectNode Parent {get; internal set;}
       
        public virtual View CurrentView {
            get {
                if (Parent == null) {
                    return Root._view.GetChild(Selector);
                }
                return Parent.CurrentView.GetChild(Selector);
            }
        }

        protected string FullPath {
            get {
                if (Parent != null) {
                    return Parent.FullPath + "//" + Selector;
                }
                return Selector.ToString();
            }
        }

        public Lazy<IDictionary<string, IValue>> Metadata {
            get {
                return _metadata;
            }
        }

        private int _indexValue;

        internal int IndexValue { get {
            return _indexValue++;
        }}

        public IEnumerable<string> TryGetRValueInContext(string property) {
            return CurrentView.TryGetRValueInContext(property);
        }

        public string ResolveMacrosInContext(string value, object[] items = null) {
            return CurrentView.ResolveMacrosInContext(value, items);
        }

      
        public void SaveFile(string filename) {
            var text = GetPropertySheetSource();
            File.WriteAllText(filename, text);
        }

        public string GetPropertySheetSource() {
            return "";
        }

        internal IEnumerable<ToRoute> Routes {
            get {
                return Keys.Select(key => (ToRoute)(() => new View(key, this[key])));
            }
        }
    }
}