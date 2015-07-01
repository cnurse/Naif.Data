//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Naif.Core.Collections;
using Naif.Core.Contracts;

namespace Naif.Data.EntityFramework
{
    public class EFLinqRepository<T> : ILinqRepository<T> where T : class
    {
        private readonly NaifDbContext _context;
        private readonly IDbSet<T> _dbSet;

        public EFLinqRepository(IUnitOfWork unitOfWork)
        {
            Requires.NotNull("unitOfWork", unitOfWork);

            var efUnitOfWork = unitOfWork as EFUnitOfWork;
            if (efUnitOfWork == null)
            {
                throw new Exception("Must be EFUnitOfWork"); // TODO: Typed exception
            }
            _context = efUnitOfWork.DbContext();
            _dbSet = _context.Set<T>();
        }

        public void Add(T item)
        {
            _dbSet.Add(item);
        }

        public void Delete(T item)
        {
            _dbSet.Attach(item);
            _dbSet.Remove(item);
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public IPagedList<T> Find(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).InPagesOf(pageSize).GetPage(pageIndex);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public IPagedList<T> GetPage(int pageIndex, int pageSize)
        {
            return _dbSet.InPagesOf(pageSize).GetPage(pageIndex);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.SingleOrDefault(predicate);
        }

        public void Update(T item)
        {
            _dbSet.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
