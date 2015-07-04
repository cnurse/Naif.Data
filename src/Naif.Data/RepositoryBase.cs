//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.Core.Contracts;
using Naif.Data.ComponentModel;

namespace Naif.Data
{
    public abstract class RepositoryBase<TModel> : IRepository<TModel> where TModel : class
    {
        private ICacheProvider _cache;

        protected RepositoryBase(ICacheProvider cache)
        {
            Requires.NotNull("cache", cache);

            _cache = cache;

            Initialize();

        }

        protected string CacheKey { get; private set; }

        protected bool IsCacheable { get; private set; }

        protected bool IsScoped { get; private set; }

        protected string Scope { get; private set; }

        public void Add(TModel item)
        {
            AddInternal(item);
            ClearCache(item);
        }

        public void Delete(TModel item)
        {
            DeleteInternal(item);
            ClearCache(item);
        }

        public abstract IEnumerable<TModel> Find(string sqlCondition, params object[] args);

        public abstract IPagedList<TModel> Find(int pageIndex, int pageSize, string sqlCondition, params object[] args);

        public abstract IEnumerable<TModel> Find(Expression<Func<TModel, bool>> predicate);

        public abstract IPagedList<TModel> Find(int pageIndex, int pageSize, Expression<Func<TModel, bool>> predicate);

        public IEnumerable<TModel> Get<TScopeType>(TScopeType scopeValue)
        {
            CheckIfScoped();

            return IsCacheable
                ? _cache.GetCachedObject(String.Format(CacheKey, scopeValue), () => GetByScopeInternal(scopeValue))
                : GetByScopeInternal(scopeValue);
        }

        public IEnumerable<TModel> GetAll()
        {
            IEnumerable<TModel> items = IsCacheable && !IsScoped
                                    ? _cache.GetCachedObject(CacheKey, GetAllInternal)
                                    : GetAllInternal();
            return items;
        }

        public TModel GetById<TProperty>(TProperty id)
        {
            TModel item = IsCacheable && !IsScoped
                         ? GetAll().SingleOrDefault(t => CompareTo(GetPrimaryKey<TProperty>(t), id) == 0)
                         : GetByIdInternal(id);

            return item;
        }

        public TModel GetById<TProperty, TScopeType>(TProperty id, TScopeType scopeValue)
        {
            CheckIfScoped();

            return Get(scopeValue).SingleOrDefault(t => CompareTo(GetPrimaryKey<TProperty>(t), id) == 0);
        }

        public IPagedList<TModel> GetPage(int pageIndex, int pageSize)
        {
            return IsCacheable && !IsScoped
                ? GetAll().InPagesOf(pageSize).GetPage(pageIndex)
                : GetPageInternal(pageIndex, pageSize);
        }

        public IPagedList<TModel> GetPage<TScopeType>(TScopeType scopeValue, int pageIndex, int pageSize)
        {
            CheckIfScoped();

            return IsCacheable
                ? Get(scopeValue).InPagesOf(pageSize).GetPage(pageIndex)
                : GetPageByScopeInternal(scopeValue, pageIndex, pageSize);
        }

        public void Update(TModel item)
        {
            UpdateInternal(item);
            ClearCache(item);
        }

        
        private void CheckIfScoped()
        {
            if (!IsScoped)
            {
                throw new NotSupportedException("This method requires the model to be cacheable and have a cache scope defined");
            }
        }

        private void ClearCache(TModel item)
        {
            if (IsCacheable)
            {
                _cache.Remove(IsScoped
                                ? String.Format(CacheKey, GetScopeValue<object>(item))
                                : CacheKey);
            }
        }

        private int CompareTo<TProperty>(TProperty first, TProperty second)
        {
            Requires.IsTypeOf<IComparable>("first", first);
            Requires.IsTypeOf<IComparable>("second", second);

            var firstComparable = first as IComparable;
            var secondComparable = second as IComparable;

            // ReSharper disable PossibleNullReferenceException
            return firstComparable.CompareTo(secondComparable);
            // ReSharper restore PossibleNullReferenceException
        }

        private TProperty GetPrimaryKey<TProperty>(TModel item)
        {
            TypeInfo modelType = typeof(TModel).GetTypeInfo();

            //Get the primary key
            var primaryKeyName = Util.GetPrimaryKeyName(modelType);

            return GetPropertyValue<TProperty>(modelType, item, primaryKeyName);
        }

        private TProperty GetPropertyValue<TProperty>(TypeInfo modelType, TModel item, string propertyName)
        {
            var property = modelType.DeclaredProperties.SingleOrDefault(p => p.Name == propertyName);

            return (TProperty)property.GetValue(item, null);
        }

        private TProperty GetPropertyValue<TProperty>(TModel item, string propertyName)
        {
            return GetPropertyValue<TProperty>(typeof(TModel).GetTypeInfo(), item, propertyName);
        }

        private TProperty GetScopeValue<TProperty>(TModel item)
        {
            return GetPropertyValue<TProperty>(item, Scope);
        }

        private void Initialize()
        {
            var type = typeof(TModel);
            var typeInfo = type.GetTypeInfo();

            Scope = Util.GetScope(type.GetTypeInfo());
            IsScoped = (!String.IsNullOrEmpty(Scope));

            IsCacheable = Util.IsCacheable(typeInfo);
            CacheKey = Util.GetCacheKey(typeInfo);
            if (IsScoped)
            {
                CacheKey += "_" + Scope + "_{0}";
            }
        }


        protected abstract void AddInternal(TModel item);

        protected abstract void DeleteInternal(TModel item);

        protected abstract IEnumerable<TModel> GetAllInternal();

        protected abstract TModel GetByIdInternal(object id);

        protected abstract IEnumerable<TModel> GetByScopeInternal(object scopeValue);

        protected abstract IPagedList<TModel> GetPageByScopeInternal(object scopeValue, int pageIndex, int pageSize);

        protected abstract IPagedList<TModel> GetPageInternal(int pageIndex, int pageSize);

        protected abstract void UpdateInternal(TModel item);


        [Obsolete("Deprecated in version 1.2.0. Use one of the Find methods which provide more flexibility")]
        public IEnumerable<TModel> GetByProperty<TProperty>(string propertyName, TProperty propertyValue)
        {
            IEnumerable<TModel> items = IsCacheable && !IsScoped
                                    ? GetAll().Where(t => CompareTo(GetPropertyValue<TProperty>(t, propertyName), propertyValue) == 0)
                                    : GetByPropertyInternal(propertyName, propertyValue);
            return items;
        }

        [Obsolete("Deprecated in version 1.2.0. Use one of the Find methods which provide more flexibility")]
        protected abstract IEnumerable<TModel> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue);
    }
}
