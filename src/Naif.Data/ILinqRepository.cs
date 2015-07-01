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
using Naif.Core.Collections;

namespace Naif.Data
{
    public interface ILinqRepository<T>
    {
        /// <summary>
        /// Add an Item into the repository
        /// </summary>
        /// <param name="item">The item to be added</param>
        void Add(T item);

        /// <summary>
        /// Delete an Item from the repository
        /// </summary>
        /// <param name="item">The item to be deleted</param>
        void Delete(T item);

        /// <summary>
        /// Find items from the repository based on a Linq predicate
        /// </summary>
        /// <param name="predicate">The Linq predicate"</param>
        /// <returns>A list of items</returns>
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Find a Page of items from the repository based on a Linq predicate
        /// </summary>
        /// <param name="pageIndex">The page Index to fetch</param>
        /// <param name="pageSize">The size of the page to fetch</param>
        /// <param name="predicate">The Linq predicate"</param>
        /// <returns>A list of items</returns>
        IPagedList<T> Find(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Returns all the items in the repository as an enumerable list
        /// </summary>
        /// <returns>The list of items</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Returns a page of items in the repository as a paged list
        /// </summary>
        /// <param name="pageIndex">The page Index to fetch</param>
        /// <param name="pageSize">The size of the page to fetch</param>
        /// <returns>The list of items</returns>
        IPagedList<T> GetPage(int pageIndex, int pageSize);

        /// <summary>
        /// Updates an Item in the repository
        /// </summary>
        /// <param name="item">The item to be updated</param>
        void Update(T item);

        ///// <summary>
        ///// Returns an enumerable list of items filtered by scope
        ///// </summary>
        ///// <remarks>
        ///// This overload should be used to get a list of items for a specific module 
        ///// instance or for a specific portal dependening on how the items in the repository 
        ///// are scoped.
        ///// </remarks>
        ///// <typeparam name="TScopeType">The type of the scope field</typeparam>
        ///// <param name="scopeValue">The value of the scope to filter by</param>
        ///// <returns>The list of items</returns>
        //IEnumerable<T> Get<TScopeType>(TScopeType scopeValue);

        ///// <summary>
        ///// Get an individual item based on the items Id field
        ///// </summary>
        ///// <typeparam name="TProperty">The type of the Id field</typeparam>
        ///// <param name="id">The value of the Id field</param>
        ///// <returns>An item</returns>
        //T GetById<TProperty>(TProperty id);

        ///// <summary>
        ///// Get an individual item based on the items Id field
        ///// </summary>
        ///// <remarks>
        ///// This overload should be used to get an item for a specific module
        ///// instance or for a specific portal dependening on how the items in the repository 
        ///// are scoped. This will allow the relevant cache to be searched first.
        ///// </remarks>
        ///// <typeparam name="TProperty">The type of the Id field</typeparam>
        ///// <param name="id">The value of the Id field</param>
        ///// <typeparam name="TScopeType">The type of the scope field</typeparam>
        ///// <param name="scopeValue">The value of the scope to filter by</param>
        ///// <returns>An item</returns>
        //T GetById<TProperty, TScopeType>(TProperty id, TScopeType scopeValue);

        ///// <summary>
        ///// Returns a page of items in the repository as a paged list filtered by scope
        ///// </summary>
        ///// <remarks>
        ///// This overload should be used to get a list of items for a specific module 
        ///// instance or for a specific portal dependening on how the items in the repository 
        ///// are scoped.
        ///// </remarks>
        ///// <typeparam name="TScopeType">The type of the scope field</typeparam>
        ///// <param name="scopeValue">The value of the scope to filter by</param>
        ///// <param name="pageIndex">The page Index to fetch</param>
        ///// <param name="pageSize">The size of the page to fetch</param>
        ///// <returns>The list of items</returns>
        //IPagedList<T> GetPage<TScopeType>(TScopeType scopeValue, int pageIndex, int pageSize);

    }
}
