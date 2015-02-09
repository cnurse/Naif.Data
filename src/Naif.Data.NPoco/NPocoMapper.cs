//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
//                                         *
// *****************************************

using System;
using System.Reflection;
using Naif.Data.ComponentModel;
using NPoco;
using TableNameAttribute = Naif.Core.ComponentModel.TableNameAttribute;

namespace Naif.Data.NPoco
{
    public class NPocoMapper : IMapper
    {
        private readonly string _tablePrefix;

        public NPocoMapper(string tablePrefix)
        {
            _tablePrefix = tablePrefix;
        }

        #region Implementation of IMapper

        public TableInfo GetTableInfo(Type pocoType)
        {
            var ti = TableInfo.FromPoco(pocoType);

            //Table Name
            ti.TableName = Util.GetAttributeValue<TableNameAttribute, string>(pocoType, "TableName", ti.TableName + "s");

            ti.TableName = _tablePrefix + ti.TableName;

            //Primary Key
            ti.PrimaryKey = Util.GetPrimaryKeyName(pocoType);

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

        #endregion
    }
}