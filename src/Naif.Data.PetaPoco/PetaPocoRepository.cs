//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.Core.Contracts;
using PetaPoco;

namespace Naif.Data.PetaPoco
{
    public class PetaPocoRepository<T> : RepositoryBase<T> where T : class
    {
        private readonly Database _database;

        public PetaPocoRepository(Database database, ICacheProvider cache)
            : this(database, cache, new PetaPocoMapper(String.Empty))
        {
        }

        public PetaPocoRepository(Database database, ICacheProvider cache, IMapper mapper)
            : base(cache)
        {
            Requires.NotNull("database", database);

            _database = database;

            if (Mappers.GetMapper(typeof(T)) is StandardMapper)
            {
                Mappers.Register(typeof(T), mapper);
            }
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

        protected override T GetByIdInternal<TProperty>(TProperty id)
        {
            return _database.SingleOrDefault<T>(id);
        }

        protected override IEnumerable<T> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue)
        {
            return _database.Query<T>(String.Format("WHERE {0} = @0", propertyName), propertyValue);
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
            _database.Update(item);
        }
    }
}
