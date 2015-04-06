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
    public class TableNameAttribute : Attribute
    {
        public string TableName { get; set; }
    }
}
