﻿//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Reflection;
using Naif.Data.ComponentModel;
using Raven.Client;
using Raven.Client.Embedded;

namespace Naif.Data.RavenDB
{
    public class RavenDBDocumentStore
    {
        private static IDocumentStore _instance;

        public static IDocumentStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("The Document Store must be initialized before it can be used");
                }
                return _instance;
            }
        }

        public static bool IsInitialized
        {
            get
            {
                return _instance != null;
            }
        }

        public static IDocumentStore Initialize(string connectionStringName)
        {
            _instance = new EmbeddableDocumentStore
            {
                ConnectionStringName = connectionStringName
            };
            _instance.Conventions.IdentityPartsSeparator = "-";
            _instance.Conventions.FindIdentityProperty = FindIdentityProperty;
            _instance.Initialize();
            return _instance;
        }

        private static bool FindIdentityProperty(PropertyInfo propertyInfo)
        {
            return (propertyInfo.Name == Util.GetPrimaryKeyName(propertyInfo.DeclaringType.GetTypeInfo()));
        }
    }
}