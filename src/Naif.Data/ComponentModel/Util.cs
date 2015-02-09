//******************************************
//  Copyright (C) 2012-2013 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included License.txt file)        *
//                                         *
// *****************************************

using System;
using System.Linq;
using System.Reflection;

namespace Naif.Data.ComponentModel
{
    public static class Util
    {
        #region Private Members

        private static TAttribute GetAttribute<TAttribute>(MemberInfo member) where TAttribute : class
        {
            return member.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(TAttribute)) as TAttribute;
        }

        private static PropertyInfo GetProperty(Attribute attribute, string propertyName)
        {
            var type = attribute.GetType().GetTypeInfo();
            return type.DeclaredProperties.SingleOrDefault(p => p.Name == propertyName);
            
        }

        #endregion

        #region Public Members

        public static TValue GetAttributeValue<TAttribute, TValue>(MemberInfo member, string argumentName, TValue defaultValue) where TAttribute : Attribute
        {
            TValue attributeValue = defaultValue;

            var attribute = GetAttribute<TAttribute>(member) as Attribute;

            if (attribute != null)
            {
                var property = GetProperty(attribute, argumentName);
                attributeValue = (TValue)property.GetValue(attribute, null);
            }

            return attributeValue;
        }

        public static string GetCacheKey(TypeInfo type)
        {
            var cacheKey = String.Empty;
            if (IsCacheable(type))
            {
                cacheKey = GetAttributeValue<CacheableAttribute, string>(type, "CacheKey", String.Format("OR_{0}", type.Name));
            }

            return cacheKey;
        }

        public static string GetPrimaryKeyName(TypeInfo type)
        {
            return GetAttributeValue<TableNameAttribute, string>(type, "KeyField", String.Empty);
        }

        public static string GetTableName(TypeInfo type, string defaultName)
        {
            return GetAttributeValue<TableNameAttribute, string>(type, "TableName", defaultName);
        }

        public static bool IsCacheable(TypeInfo type)
        {
            return (GetAttribute<CacheableAttribute>(type) != null);
        }

        #endregion
    }
}
