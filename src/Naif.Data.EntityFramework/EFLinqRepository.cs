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
using System.Linq;
using System.Linq.Expressions;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.Core.Contracts;

namespace Naif.Data.EntityFramework
{
    public class EFLinqRepository<TModel> : LinqRepositoryBase<TModel> where TModel : class
    {
        private readonly NaifDbContext _context;
        private readonly IDbSet<TModel> _dbSet;

        public EFLinqRepository(IUnitOfWork unitOfWork, ICacheProvider cache) :base(cache)
        {
            Requires.NotNull("unitOfWork", unitOfWork);

            var efUnitOfWork = unitOfWork as EFUnitOfWork;
            if (efUnitOfWork == null)
            {
                throw new Exception("Must be EFUnitOfWork"); // TODO: Typed exception
            }
            _context = efUnitOfWork.DbContext();
            _dbSet = _context.Set<TModel>();
        }

        public override IQueryable<TModel> Find(Expression<Func<TModel, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public override IPagedList<TModel> Find(int pageIndex, int pageSize, Expression<Func<TModel, bool>> predicate)
        {
            return _dbSet.Where(predicate).InPagesOf(pageSize).GetPage(pageIndex);
        }

        protected override void AddInternal(TModel item)
        {
            _dbSet.Add(item);
        }

        protected override void DeleteInternal(TModel item)
        {
            _dbSet.Remove(item);
        }

        protected override IEnumerable<TModel> GetAllInternal()
        {
            return _dbSet;
        }

        protected override IEnumerable<TModel> GetByScopeInternal<TScopeType>(TScopeType scopeValue)
        {
            throw new NotImplementedException();
        }

        protected override IPagedList<TModel> GetPageByScopeInternal<TScopeType>(TScopeType scopeValue, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        protected override IPagedList<TModel> GetPageInternal(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        protected override void UpdateInternal(TModel item)
        {
            
        }
    }
}
