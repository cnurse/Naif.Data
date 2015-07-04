//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using Naif.Core.Caching;
using Naif.Core.Contracts;
using NPoco;

namespace Naif.Data.NPoco
{
    public class NPocoUnitOfWork : IUnitOfWork
    {
        private readonly ICacheProvider _cache;

        public NPocoUnitOfWork(string connectionStringName, ICacheProvider cache)
            : this(connectionStringName, String.Empty, cache)
        {
        }

        public NPocoUnitOfWork(string connectionStringName, string tablePrefix, ICacheProvider cache)
        {
            Requires.NotNullOrEmpty("connectionStringName", connectionStringName);
            Requires.NotNull("cache", cache);

            Database = new Database(connectionStringName) {Mapper = new NPocoMapper(tablePrefix)};
            _cache = cache;
        }

        public void Commit() { }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new NPocoRepository<T>(this, _cache);
        }

        internal Database Database { get; private set; }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}