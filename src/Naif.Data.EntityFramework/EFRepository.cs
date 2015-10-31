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
    public class EFRepository<TModel> : RepositoryBase<TModel> where TModel : class
    {
        private readonly NaifDbContext _context;
        private readonly IDbSet<TModel> _dbSet;

        public EFRepository(IUnitOfWork unitOfWork, ICacheProvider cache) :base(cache)
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

        public override IEnumerable<TModel> Find(string sqlCondition, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override IPagedList<TModel> Find(int pageIndex, int pageSize, string sqlCondition, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TModel> Find(Expression<Func<TModel, bool>> predicate)
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

        protected override TModel GetByIdInternal(object id)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<TModel> GetByScopeInternal(object scopeValue)
        {
            throw new NotImplementedException();
        }

        protected override IPagedList<TModel> GetPageByScopeInternal(object scopeValue, int pageIndex, int pageSize)
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

        protected override IEnumerable<TModel> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue)
        {
            throw new NotImplementedException();
        }
    }
}
