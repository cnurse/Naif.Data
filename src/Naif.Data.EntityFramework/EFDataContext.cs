//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Data.Entity;
using System.Runtime.InteropServices.ComTypes;
using Naif.Core.Caching;
using Naif.Core.Contracts;

namespace Naif.Data.EntityFramework
{
    public class EFDataContext : IDataContext
    {
        private readonly ICacheProvider _cache;
        private readonly NaifDbContext _context;
        private DbContextTransaction _transaction;

        public EFDataContext(string connectionString, ICacheProvider cache, Action<DbModelBuilder> modelCreateCallback)
             : this(connectionString, String.Empty, cache, modelCreateCallback)
        {
        }

        public EFDataContext(string connectionString, string tablePrefix, ICacheProvider cache, Action<DbModelBuilder> modelCreateCallback)
        {
            Requires.NotNullOrEmpty("connectionString", connectionString);
            Requires.NotNull("cache", cache);

            _context = new NaifDbContext(connectionString, modelCreateCallback);
            _cache = cache;
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new EFRepository<T>(_context, _cache);
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void RollbackTransaction()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
