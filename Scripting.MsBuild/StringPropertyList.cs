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

namespace ClrPlus.Scripting.MsBuild {
    using System;
    using System.Linq;
    using Core.Collections;

    public class StringPropertyList : ObservableList<string> {
        public StringPropertyList(Func<string> getter, Action<string> setter) {
            var initial = getter();
            if (!string.IsNullOrEmpty(initial)) {
                foreach (var i in initial.Split(';')) {
                    Add(i);    
                }
            }

            ListChanged += (source, args) => setter(this.Reverse().Aggregate((current, each) => current + ";" + each));
        }
    }
}