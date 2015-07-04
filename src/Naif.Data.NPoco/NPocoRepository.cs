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
using System.Reflection;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.Core.Contracts;
using Naif.Data.ComponentModel;
using NPoco;
// ReSharper disable UseStringInterpolation

namespace Naif.Data.NPoco
{
    public class NPocoRepository<TModel> : RepositoryBase<TModel> where TModel : class
    {
        private readonly Database _database;

        public NPocoRepository(IUnitOfWork unitOfWork, ICacheProvider cache) : base(cache)
        {
            Requires.NotNull("unitOfWork", unitOfWork);

            var nPocoUnitOfWork = unitOfWork as NPocoUnitOfWork;
            if (nPocoUnitOfWork == null)
            {
                throw new Exception("Must be NPocoUnitOfWork"); // TODO: Typed exception
            }
            _database = nPocoUnitOfWork.Database;
        }

        public override IEnumerable<TModel> Find(string sqlCondition, params object[] args)
        {
            return _database.Fetch<TModel>(sqlCondition, args);
        }

        public override IPagedList<TModel> Find(int pageIndex, int pageSize, string sqlCondition, params object[] args)
        {
            //Make sure that the sql Condition contains an ORDER BY Clause
            if (!sqlCondition.ToUpperInvariant().Contains("ORDER BY"))
            {
                sqlCondition = String.Format("{0} ORDER BY {1}", sqlCondition, Util.GetPrimaryKeyName(typeof(TModel).GetTypeInfo()));
            }
            Page<TModel> petaPocoPage = _database.Page<TModel>(pageIndex + 1, pageSize, sqlCondition, args);

            return new PagedList<TModel>(petaPocoPage.Items, (int)petaPocoPage.TotalItems, pageIndex, pageSize);
        }

        public override IEnumerable<TModel> Find(Expression<Func<TModel, bool>> predicate)
        {
            return _database.FetchWhere(predicate);
        }

        public override IPagedList<TModel> Find(int pageIndex, int pageSize, Expression<Func<TModel, bool>> predicate)
        {
            return Find(predicate).InPagesOf(pageSize).GetPage(pageIndex);
        }

        protected override void AddInternal(TModel item)
        {
            _database.Insert(item);
        }

        protected override void DeleteInternal(TModel item)
        {
            _database.Delete(item);
        }

        protected override IEnumerable<TModel> GetAllInternal()
        {
            return _database.Fetch<TModel>("");
        }

        protected override TModel GetByIdInternal(object id)
        {
            return _database.SingleOrDefault<TModel>(String.Format("WHERE {0} = @0", Util.GetPrimaryKeyName(typeof(TModel).GetTypeInfo())), id);
        }

        protected override IEnumerable<TModel> GetByScopeInternal(object scopeValue)
        {
            return _database.Fetch<TModel>(GetScopeSql(), scopeValue);
        }

        protected override IPagedList<TModel> GetPageInternal(int pageIndex, int pageSize)
        {
            return Find(pageIndex, pageSize, String.Empty);
        }

        protected override IPagedList<TModel> GetPageByScopeInternal(object propertyValue, int pageIndex, int pageSize)
        {
            return Find(pageIndex, pageSize, GetScopeSql(), propertyValue);
        }

        private string GetScopeSql()
        {
            return String.Format("WHERE {0} = @0", Util.GetColumnName(typeof(TModel).GetTypeInfo(), Scope));
        }

        protected override void UpdateInternal(TModel item)
        {
            _database.Update(item);
        }


        [Obsolete("Deprecated in version 1.2.0. Use one of the Find methods which provide more flexibility")]
        protected override IEnumerable<TModel> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue)
        {
            return _database.Query<TModel>(String.Format("WHERE {0} = @0", propertyName), propertyValue);
        }
    }
}
