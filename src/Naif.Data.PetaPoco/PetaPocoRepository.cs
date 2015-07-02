//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.Core.Contracts;
using Naif.Data.ComponentModel;
using PetaPoco;

namespace Naif.Data.PetaPoco
{
    public class PetaPocoRepository<T> : RepositoryBase<T> where T : class
    {
        private readonly Database _database;

        public PetaPocoRepository(IUnitOfWork unitOfWork, ICacheProvider cache) : base(cache)
        {
            Requires.NotNull("unitOfWork", unitOfWork);

            var petaPocoUnitOfWork = unitOfWork as PetaPocoUnitOfWork;
            if (petaPocoUnitOfWork == null)
            {
                throw new Exception("Must be PocoUnitOfWork"); // TODO: Typed exception
            }

            _database = petaPocoUnitOfWork.Database;

            if (Mappers.GetMapper(typeof(T)) is StandardMapper)
            {
                Mappers.Register(typeof(T), petaPocoUnitOfWork.Mapper);
            }
        }

        public override IEnumerable<T> Find(string sqlCondition, params object[] args)
        {
            return _database.Fetch<T>(sqlCondition, args);
        }

        public override IPagedList<T> Find(int pageIndex, int pageSize, string sqlCondition, params object[] args)
        {
            //Make sure that the sql Condition contains an ORDER BY Clause
            if (!sqlCondition.ToUpperInvariant().Contains("ORDER BY"))
            {
                sqlCondition = String.Format("{0} ORDER BY {1}", sqlCondition, Util.GetPrimaryKeyName(typeof(T).GetTypeInfo()));
            }
            Page<T> petaPocoPage = _database.Page<T>(pageIndex + 1, pageSize, sqlCondition, args);

            return new PagedList<T>(petaPocoPage.Items, (int)petaPocoPage.TotalItems, pageIndex, pageSize);
        }

        protected override void AddInternal(T item)
        {
            _database.Insert(item);
        }

        protected override void DeleteInternal(T item)
        {
            _database.Delete(item);
        }

        protected override IEnumerable<T> GetAllInternal()
        {
            return _database.Fetch<T>("");
        }

        protected override T GetByIdInternal(object id)
        {
            return _database.SingleOrDefault<T>(id);
        }

        protected override IEnumerable<T> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue)
        {
            return _database.Query<T>(String.Format("WHERE {0} = @0", propertyName), propertyValue);
        }

        protected override IEnumerable<T> GetByScopeInternal(object propertyValue)
        {
            return _database.Fetch<T>(GetScopeSql(), propertyValue);
        }

        protected override IPagedList<T> GetPageInternal(int pageIndex, int pageSize)
        {
            return Find(pageIndex, pageSize, String.Empty);
        }

        protected override IPagedList<T> GetPageByScopeInternal(object propertyValue, int pageIndex, int pageSize)
        {
            return Find(pageIndex, pageSize, GetScopeSql(), propertyValue);
        }

        private string GetScopeSql()
        {
            return String.Format("WHERE {0} = @0", Util.GetColumnName(typeof(T).GetTypeInfo(), Scope));
        }

        protected override void UpdateInternal(T item)
        {
            _database.Update(item);
        }
    }
}
