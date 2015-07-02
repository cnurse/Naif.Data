﻿//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Data.Entity;
using Naif.Core.Contracts;

namespace Naif.Data.EntityFramework
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly NaifDbContext _dbContext;

        public EFUnitOfWork(string connectionString, Action<DbModelBuilder> modelCreateCallback)
        {
            Requires.NotNullOrEmpty("connectionString", connectionString);

            _dbContext = new NaifDbContext(connectionString, modelCreateCallback);
        }

        public EFUnitOfWork(NaifDbContext dbContext)
        {
            Requires.NotNull("dbContext", dbContext);

            _dbContext = dbContext;
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
            return new EFLinqRepository<T>(this);
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