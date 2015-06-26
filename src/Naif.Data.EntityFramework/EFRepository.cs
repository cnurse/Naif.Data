//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using System.Data.Entity;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.Core.Contracts;

namespace Naif.Data.EntityFramework
{
    public class EFRepository<T> : RepositoryBase<T> where T : class
    {
        private readonly NaifDbContext _context;
        private readonly IDbSet<T> _dbSet;

        public EFRepository(NaifDbContext context, ICacheProvider cache)
            : base(cache)
        {
            Requires.NotNull("context", context);
            Requires.NotNull("cache", cache);

            _context = context;
            _dbSet = _context.Set<T>();
        }

        public override IEnumerable<T> Find(string sqlCondition, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override IPagedList<T> Find(int pageIndex, int pageSize, string sqlCondition, params object[] args)
        {
            throw new NotImplementedException();
        }

        protected override void AddInternal(T item)
        {
            _dbSet.Add(item);
            _context.SaveChanges();
        }

        protected override void DeleteInternal(T item)
        {
            if (_context.Entry(item).State == EntityState.Detached)
            {
                _dbSet.Attach(item);
            }
            _dbSet.Remove(item);
            _context.SaveChanges();
        }

        protected override IEnumerable<T> GetAllInternal()
        {
            return _dbSet;
        }

        protected override T GetByIdInternal<TProperty>(TProperty id)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<T> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<T> GetByScopeInternal(object propertyValue)
        {
            throw new NotImplementedException();
        }

        protected override IPagedList<T> GetPageInternal(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        protected override IPagedList<T> GetPageByScopeInternal(object propertyValue, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateInternal(T item)
        {
            _dbSet.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
