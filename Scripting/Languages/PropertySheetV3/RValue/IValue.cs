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

namespace ClrPlus.Scripting.Languages.PropertySheetV3.RValue {
    using System.Collections.Generic;

    public interface IValue {
        string Value {get;}
        IEnumerable<string> Values { get; }
        IValueContext Context {get;set;}
    }

    public interface IValueContext {
        string ResolveMacrosInContext(string value, object[] items);
        IEnumerable<string> TryGetRValueInContext(string property);
    }
}