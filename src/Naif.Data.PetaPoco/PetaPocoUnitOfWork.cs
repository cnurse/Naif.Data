//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.CodeDom;
using Naif.Core.Caching;
using Naif.Core.Contracts;
using PetaPoco;

namespace Naif.Data.PetaPoco
{
    public class PetaPocoUnitOfWork : IUnitOfWork
    {
        public PetaPocoUnitOfWork(string connectionStringName)
            : this(connectionStringName, String.Empty)
        {
        }

        public PetaPocoUnitOfWork(string connectionStringName, string tablePrefix)
        {
            Requires.NotNullOrEmpty("connectionStringName", connectionStringName);

            Database = new Database(connectionStringName);
            Mapper = new PetaPocoMapper(tablePrefix);
        }

        public void Commit() { }

        internal Database Database { get; private set; }

        internal IMapper Mapper { get; private set; }

        public bool SupportsLinq
        {
            get { return false; }
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}