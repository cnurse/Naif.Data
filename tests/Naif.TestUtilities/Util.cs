//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Reflection;

namespace Naif.TestUtilities
{
    public static class Util
    {

        public static object GetPrivateMember<TInstance, TField>(TInstance instance, string fieldName)
        {
            Type type = typeof(TInstance);

            const BindingFlags privateBindings = BindingFlags.NonPublic | BindingFlags.Instance;

            // retrive private field from class
            FieldInfo field = type.GetField(fieldName, privateBindings);

            return (TField)field.GetValue(instance);
        }

    }
}
