//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System.Collections.Generic;

namespace Naif.Data
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        T GetById<TProperty>(TProperty id);

        IEnumerable<T> GetByProperty<TProperty>(string propertyName, TProperty propertyValue);

        void Add(T item);

        void Delete(T item);

        void Update(T item);
    }
}


