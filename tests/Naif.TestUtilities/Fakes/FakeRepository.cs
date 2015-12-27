//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.Data;

namespace Naif.TestUtilities.Fakes
{
    public class FakeRepository<TModel> : RepositoryBase<TModel> where TModel : class
    {
        public FakeRepository(ICacheProvider cache) : base(cache)
        {
        }

        public override IEnumerable<TModel> Find(string sqlCondition, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public override IPagedList<TModel> Find(int pageIndex, int pageSize, string sqlCondition, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<TModel> Find(Expression<Func<TModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public override IPagedList<TModel> Find(int pageIndex, int pageSize, Expression<Func<TModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        protected override void AddInternal(TModel item)
        {
            throw new System.NotImplementedException();
        }

        protected override void DeleteInternal(TModel item)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<TModel> GetAllInternal()
        {
            throw new System.NotImplementedException();
        }

        protected override TModel GetByIdInternal(object id)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<TModel> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<TModel> GetByScopeInternal(object propertyValue)
        {
            throw new System.NotImplementedException();
        }

        protected override IPagedList<TModel> GetPageInternal(int pageIndex, int pageSize)
        {
            throw new System.NotImplementedException();
        }

        protected override IPagedList<TModel> GetPageByScopeInternal(object propertyValue, int pageIndex, int pageSize)
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateInternal(TModel item)
        {
            throw new System.NotImplementedException();
        }
    }
}
