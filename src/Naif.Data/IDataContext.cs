//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
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