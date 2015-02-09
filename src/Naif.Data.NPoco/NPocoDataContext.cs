//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
//                                         *
// *****************************************

using System;
using Naif.Core.Caching;
using Naif.Core.Contracts;
using NPoco;

namespace Naif.Data.NPoco
{
    public class NPocoDataContext : IDataContext
    {
        #region Private Members

        private readonly ICacheProvider _cache;
        private readonly Database _database;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public NPocoDataContext(string connectionString, ICacheProvider cache)
            : this(connectionString, String.Empty, cache)
        {
        }

        public NPocoDataContext(string connectionString, string tablePrefix, ICacheProvider cache)
        {
            Requires.NotNullOrEmpty("connectionString", connectionString);
            Requires.NotNull("cache", cache);

            _database = new Database(connectionString, "System.Data.SqlClient");
            _cache = cache;
            _mapper = new NPocoMapper(tablePrefix);
        }

        #endregion

        #region Implementation of IDataContext

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
            return new NPocoRepository<T>(_database, _cache, _mapper);
        }

        public void RollbackTransaction()
        {
            _database.AbortTransaction();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _database.Dispose();
        }
        #endregion
    }
}