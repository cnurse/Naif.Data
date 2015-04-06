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
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute()
        {
            AutoIncrement = true;
        }

        public bool AutoIncrement { get; set; }
        public string KeyField { get; set; }

    }
}
