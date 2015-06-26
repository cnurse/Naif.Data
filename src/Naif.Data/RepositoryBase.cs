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
using System.Reflection;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.Core.Contracts;
using Naif.Data.ComponentModel;

namespace Naif.Data
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        private readonly ICacheProvider _cache;

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

        public void Add(T item)
        {
            AddInternal(item);
            ClearCache(item);
        }

        public void Delete(T item)
        {
            DeleteInternal(item);
            ClearCache(item);
        }

        public abstract IEnumerable<T> Find(string sqlCondition, params object[] args);

        public abstract IPagedList<T> Find(int pageIndex, int pageSize, string sqlCondition, params object[] args);

        public IEnumerable<T> Get<TScopeType>(TScopeType scopeValue)
        {
            CheckIfScoped();

            if (IsCacheable)
            {
                CacheKey = String.Format(CacheKey, scopeValue);
            }

            return IsCacheable
                ? _cache.GetCachedObject<IEnumerable<T>>(CacheKey, () => GetByScopeInternal(scopeValue))
                : GetByScopeInternal(scopeValue);
        }

        public IEnumerable<T> GetAll()
        {
            IEnumerable<T> items = IsCacheable && !IsScoped
                                    ? _cache.GetCachedObject(CacheKey, GetAllInternal)
                                    : GetAllInternal();
            return items;
        }

        //TODO Write Unit Tests
        public T GetById<TProperty>(TProperty id)
        {
            T item = IsCacheable && !IsScoped
                         ? GetAll().SingleOrDefault(t => CompareTo(GetPrimaryKey<TProperty>(t), id) == 0)
                         : GetByIdInternal(id);

            return item;
        }

        //TODO Write Unit Tests
        public T GetById<TProperty, TScopeType>(TProperty id, TScopeType scopeValue)
        {
            CheckIfScoped();

            return Get(scopeValue).SingleOrDefault(t => CompareTo(GetPrimaryKey<TProperty>(t), id) == 0);
        }

        //TODO Write Unit Tests
        public IPagedList<T> GetPage(int pageIndex, int pageSize)
        {
            return IsCacheable && !IsScoped
                ? GetAll().InPagesOf(pageSize).GetPage(pageIndex)
                : GetPageInternal(pageIndex, pageSize);
        }

        //TODO Write Unit Tests
        public IPagedList<T> GetPage<TScopeType>(TScopeType scopeValue, int pageIndex, int pageSize)
        {
            CheckIfScoped();

            return IsCacheable
                ? Get(scopeValue).InPagesOf(pageSize).GetPage(pageIndex)
                : GetPageByScopeInternal(scopeValue, pageIndex, pageSize);
        }

        //TODO Write Unit Tests
        public void Update(T item)
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

        private void ClearCache(T item)
        {
            if (IsCacheable)
            {
                _cache.Remove(IsScoped
                                ? String.Format(CacheKey, GetScopeValue<object>(item))
                                : CacheKey);
            }
        }

        protected int CompareTo<TProperty>(TProperty first, TProperty second)
        {
            Requires.IsTypeOf<IComparable>("first", first);
            Requires.IsTypeOf<IComparable>("second", second);

            var firstComparable = first as IComparable;
            var secondComparable = second as IComparable;

// ReSharper disable PossibleNullReferenceException
            return firstComparable.CompareTo(secondComparable);
// ReSharper restore PossibleNullReferenceException
        }

        protected TProperty GetPrimaryKey<TProperty>(T item)
        {
            TypeInfo modelType = typeof(T).GetTypeInfo();

            //Get the primary key
            var primaryKeyName = Util.GetPrimaryKeyName(modelType);

            return GetPropertyValue<TProperty>(modelType, item, primaryKeyName);
        }

        private TProperty GetPropertyValue<TProperty>(TypeInfo modelType, T item, string propertyName)
        {
            var property = modelType.DeclaredProperties.SingleOrDefault(p => p.Name == propertyName);

            return (TProperty)property.GetValue(item, null);
        }

        protected TProperty GetPropertyValue<TProperty>(T item, string propertyName)
        {
            return GetPropertyValue<TProperty>(typeof(T).GetTypeInfo(), item, propertyName);
        }

        protected TProperty GetScopeValue<TProperty>(T item)
        {
            return GetPropertyValue<TProperty>(item, Scope);
        }

        private void Initialize()
        {
            var type = typeof (T);
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


        protected abstract void AddInternal(T item);

        protected abstract void DeleteInternal(T item);

        protected abstract IEnumerable<T> GetAllInternal();

        protected abstract T GetByIdInternal(object id);

        protected abstract IEnumerable<T> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue);

        //TODO Write Unit Tests
        protected abstract IEnumerable<T> GetByScopeInternal(object propertyValue);

        //TODO Write Unit Tests
        protected abstract IPagedList<T> GetPageInternal(int pageIndex, int pageSize);

        //TODO Write Unit Tests
        protected abstract IPagedList<T> GetPageByScopeInternal(object propertyValue, int pageIndex, int pageSize);

        protected abstract void UpdateInternal(T item);

        [ObsoleteAttribute("Deprecated in version 1.2.0. Use one of the Find methods which provide more flexibility")]
        public IEnumerable<T> GetByProperty<TProperty>(string propertyName, TProperty propertyValue)
        {
            IEnumerable<T> items = IsCacheable && !IsScoped
                                    ? GetAll().Where(t => CompareTo(GetPropertyValue<TProperty>(t, propertyName), propertyValue) == 0)
                                    : GetByPropertyInternal(propertyName, propertyValue);
            return items;
        }
    }
}
