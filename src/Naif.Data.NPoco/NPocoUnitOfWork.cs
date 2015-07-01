//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using Naif.Core.Contracts;
using NPoco;

namespace Naif.Data.NPoco
{
    public class NPocoUnitOfWork : IUnitOfWork
    {
        public NPocoUnitOfWork(string connectionStringName)
            : this(connectionStringName, String.Empty)
        {
        }

        public NPocoUnitOfWork(string connectionStringName, string tablePrefix)
        {
            Requires.NotNullOrEmpty("connectionStringName", connectionStringName);

            Database = new Database(connectionStringName) {Mapper = new NPocoMapper(tablePrefix)};
        }

        public void Commit() { }

        internal Database Database { get; private set; }

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