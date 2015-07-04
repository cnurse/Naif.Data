//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.CodeDom;
using Naif.Core.Caching;
using Naif.Core.Contracts;
using PetaPoco;

namespace Naif.Data.PetaPoco
{
    public class PetaPocoUnitOfWork : IUnitOfWork
    {
        private readonly ICacheProvider _cache;

        public PetaPocoUnitOfWork(string connectionStringName, ICacheProvider cache)
            : this(connectionStringName, String.Empty, cache)
        {
        }

        public PetaPocoUnitOfWork(string connectionStringName, string tablePrefix, ICacheProvider cache)
        {
            Requires.NotNullOrEmpty("connectionStringName", connectionStringName);
            Requires.NotNull("cache", cache);

            Database = new Database(connectionStringName);
            Mapper = new PetaPocoMapper(tablePrefix);
            _cache = cache;
        }

        public void Commit() { }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new PetaPocoRepository<T>(this, _cache);
        }

        internal Database Database { get; private set; }

        internal IMapper Mapper { get; private set; }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}