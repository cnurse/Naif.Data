//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
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


