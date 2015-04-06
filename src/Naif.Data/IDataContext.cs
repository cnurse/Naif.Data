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
    public interface IDataContext : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;

        void BeginTransaction();

        void Commit();

        void RollbackTransaction();
    }
}