//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using Naif.Data.ComponentModel;

namespace Naif.TestUtilities.Models
{
    [Cacheable(TestConstants.CACHE_CatsKey)]
    [Scope(TestConstants.CACHE_ScopeModule)]
    public class CacheableCat
    {
        public int? Age { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }
    }
}
