//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;

namespace Naif.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        bool SupportsLinq { get; }
    }
}