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
using PetaPoco;

namespace Naif.Data.PetaPoco
{
    public class PetaPocoDataContext : IDataContext
    {
        private readonly ICacheProvider _cache;
        private readonly Database _database;
        private readonly IMapper _mapper;

        public PetaPocoDataContext(string connectionString, ICacheProvider cache)
            : this(connectionString, String.Empty, cache)
        {
        }

        public PetaPocoDataContext(string connectionString, string tablePrefix, ICacheProvider cache)
        {
            Requires.NotNullOrEmpty("connectionString", connectionString);
            Requires.NotNull("cache", cache);

            _database = new Database(connectionString, "System.Data.SqlClient");
            _cache = cache;
            _mapper = new PetaPocoMapper(tablePrefix);
        }

        public void BeginTransaction()
        {
            _database.BeginTransaction();
        }

        public void Commit()
        {
            _database.CompleteTransaction();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new PetaPocoRepository<T>(_database, _cache, _mapper);
        }

        public void RollbackTransaction()
        {
            _database.AbortTransaction();
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}