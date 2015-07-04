//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Data.Entity;
using Naif.Core.Caching;
using Naif.Core.Contracts;

namespace Naif.Data.EntityFramework
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly ICacheProvider _cache;
        private readonly NaifDbContext _dbContext;

        public EFUnitOfWork(string connectionString, Action<DbModelBuilder> modelCreateCallback, ICacheProvider cache)
        {
            Requires.NotNull(cache);
            Requires.NotNullOrEmpty("connectionString", connectionString);

            _dbContext = new NaifDbContext(connectionString, modelCreateCallback);
            _cache = cache;
        }

        public EFUnitOfWork(NaifDbContext dbContext, ICacheProvider cache)
        {
            Requires.NotNull(dbContext);
            Requires.NotNull(cache);

            _dbContext = dbContext;
            _cache = cache;
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public ILinqRepository<T> GetLinqRepository<T>() where T : class
        {
            return new EFLinqRepository<T>(this, _cache);
        }

        internal NaifDbContext DbContext()
        {
            return _dbContext;
        }

        public bool SupportsLinq
        {
            get { return true; }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
