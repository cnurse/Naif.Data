//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System.Collections.Generic;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.Data;

namespace Naif.TestUtilities.Fakes
{
    public class FakeRepository<T> : RepositoryBase<T> where T : class
    {
        public FakeRepository(ICacheProvider cache) : base(cache)
        {
        }

        public override IEnumerable<T> Find(string sqlCondition, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public override IPagedList<T> Find(int pageIndex, int pageSize, string sqlCondition, params object[] args)
        {
            throw new System.NotImplementedException();
        }

        protected override void AddInternal(T item)
        {
            throw new System.NotImplementedException();
        }

        protected override void DeleteInternal(T item)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<T> GetAllInternal()
        {
            throw new System.NotImplementedException();
        }

        protected override T GetByIdInternal(object id)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<T> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue)
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<T> GetByScopeInternal(object propertyValue)
        {
            throw new System.NotImplementedException();
        }

        protected override IPagedList<T> GetPageInternal(int pageIndex, int pageSize)
        {
            throw new System.NotImplementedException();
        }

        protected override IPagedList<T> GetPageByScopeInternal(object propertyValue, int pageIndex, int pageSize)
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateInternal(T item)
        {
            throw new System.NotImplementedException();
        }
    }
}
