//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Reflection;
using Naif.Data.ComponentModel;
using NPoco;

namespace Naif.Data.NPoco
{
    public class NPocoMapper : IMapper
    {
        private readonly string _tablePrefix;

        public NPocoMapper(string tablePrefix)
        {
            _tablePrefix = tablePrefix;
        }

        public void GetTableInfo(Type t, TableInfo ti)
        {
            //Table Name
            ti.TableName = Util.GetAttributeValue<ComponentModel.TableNameAttribute, string>(t, "TableName", ti.TableName + "s");

            ti.TableName = _tablePrefix + ti.TableName;

            //Primary Key
            ti.PrimaryKey = Util.GetPrimaryKeyName(t.GetTypeInfo());

            ti.AutoIncrement = true;
        }

        public bool MapMemberToColumn(MemberInfo mi, ref string columnName, ref bool resultColumn)
        {
            var ci = ColumnInfo.FromMemberInfo(mi);

            //Column Name
            columnName = Util.GetAttributeValue<ColumnNameAttribute, string>(mi, "ColumnName", ci.ColumnName);

            return true;
        }

        public Func<object, object> GetFromDbConverter(MemberInfo mi, Type sourceType)
        {
            return null;
        }

        public Func<object, object> GetFromDbConverter(Type destType, Type sourceType)
        {
            return null;
        }

        public Func<object, object> GetParameterConverter(Type sourceType)
        {
            return null;
        }

        public Func<object, object> GetToDbConverter(Type destType, Type sourceType)
        {
            return null;
        }

        public Func<object, object> GetToDbConverter(Type destType, MemberInfo sourceMemberInfo)
        {
            return null;
        }
    }
}