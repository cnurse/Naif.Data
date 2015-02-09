//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using Naif.Core.Caching;
using Naif.Core.Contracts;
using NPoco;

namespace Naif.Data.NPoco
{
    public class NPocoRepository<T> : RepositoryBase<T> where T : class
    {
        #region Private Members

        private readonly Database _database;

        #endregion

        #region Constructors

        public NPocoRepository(Database database, ICacheProvider cache)
            : this(database, cache, new NPocoMapper(String.Empty))
        {
        }

        public NPocoRepository(Database database, ICacheProvider cache, IMapper mapper)
            : base(cache)
        {
            Requires.NotNull("database", database);

            _database = database;

            if (Mappers.GetMapper(typeof(T)) is StandardMapper)
            {
                Mappers.Register(typeof(T), mapper);
            }
        }

        #endregion

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

        protected override void UpdateInternal(T item)
        {
            _database.Update(item);
        }
    }
}
