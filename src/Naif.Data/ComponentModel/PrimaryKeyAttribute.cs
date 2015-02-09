//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
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
