//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Reflection;

using PetaPoco;
using Naif.Data.ComponentModel;

namespace Naif.Data.PetaPoco
{
    public class PetaPocoMapper : IMapper
    {
        private readonly string _tablePrefix;

        public PetaPocoMapper(string tablePrefix)
        {
            _tablePrefix = tablePrefix;
        }

        public TableInfo GetTableInfo(Type pocoType)
        {
            var ti = TableInfo.FromPoco(pocoType);

            //Table Name
            ti.TableName = Util.GetAttributeValue<ComponentModel.TableNameAttribute, string>(pocoType, "TableName", ti.TableName + "s");

            ti.TableName = _tablePrefix + ti.TableName;

            //Primary Key
            ti.PrimaryKey = Util.GetPrimaryKeyName(pocoType.GetTypeInfo());

            ti.AutoIncrement = true;

            return ti;
        }

        public ColumnInfo GetColumnInfo(PropertyInfo pocoProperty)
        {
            var ci = ColumnInfo.FromProperty(pocoProperty);

            //Column Name
            ci.ColumnName = Util.GetAttributeValue<ColumnNameAttribute, string>(pocoProperty, "ColumnName", ci.ColumnName);

            return ci;
        }

        public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type sourceType)
        {
            return null;
        }

        public Func<object, object> GetToDbConverter(PropertyInfo sourceProperty)
        {
            return null;
        }
    }
}