//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;

namespace Naif.Data.ComponentModel
{
    public class ScopeAttribute : Attribute
    {
        public ScopeAttribute(string scope)
        {
            Scope = scope;
        }

        /// <summary>
        /// The property to use to scope the cache.  The default is an empty string.
        /// </summary>
        public string Scope { get; set; }
    }
}
