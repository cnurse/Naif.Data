//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Data.Entity;
using Naif.TestUtilities.Models;

namespace Naif.Data.EntityFramework
{
    public class NaifDbContext : DbContext
    {
        private readonly Action<DbModelBuilder> _modelCreateCallback ;

        public NaifDbContext(string connectionString, Action<DbModelBuilder> modelCreateCallback) : base(connectionString)
        {
            _modelCreateCallback = modelCreateCallback;

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // ReSharper disable once UseNullPropagation
            if (_modelCreateCallback != null)
            {
                _modelCreateCallback(modelBuilder);
            }
        }
    }
}
