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
    public class CacheableAttribute : Attribute
    {
        public CacheableAttribute()
        {
        }

        public CacheableAttribute(string cacheKey)
        {
            CacheKey = cacheKey;
        }

        public string CacheKey { get; set; }
    }
}
