﻿#region License
//
// Command Line Library: ReflectionUtil.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2013 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Reflection;
#endregion

namespace CommandLine.Utils
{
    static class ReflectionUtil
    {
        static ReflectionUtil()
        {
            AssemblyFromWhichToPullInformation = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        }

        public static IList<Pair<PropertyInfo, TAttribute>> RetrievePropertyList<TAttribute>(object target)
                where TAttribute : Attribute
        {
            var key = new Pair<Type, object>(typeof(Pair<PropertyInfo, TAttribute>), target);
            var cached = ReflectionCache.Instance[key];
            if (cached == null)
            {
                IList<Pair<PropertyInfo, TAttribute>> list = new List<Pair<PropertyInfo, TAttribute>>();
                if (target != null)
                {
                    var propertiesInfo = target.GetType().GetProperties();

                    foreach (var property in propertiesInfo)
                    {
                        if (property != null && (property.CanRead && property.CanWrite))
                        {
                            var setMethod = property.GetSetMethod();
                            if (setMethod != null && !setMethod.IsStatic)
                            {
                                var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                                if (attribute != null)
                                {
                                    list.Add(new Pair<PropertyInfo, TAttribute>(property, (TAttribute)attribute));
                                }
                            }
                        }
                    }
                }
                ReflectionCache.Instance[key] = list;
                return list;
            }
            return (IList<Pair<PropertyInfo, TAttribute>>)cached;
        }

        public static Pair<MethodInfo, TAttribute> RetrieveMethod<TAttribute>(object target)
                where TAttribute : Attribute
        {
            var key = new Pair<Type, object>(typeof(Pair<MethodInfo, TAttribute>), target);
            var cached = ReflectionCache.Instance[key];
            if (cached == null)
            {
                var info = target.GetType().GetMethods();
                foreach (MethodInfo method in info)
                {
                    if (!method.IsStatic)
                    {
                        Attribute attribute =
                            Attribute.GetCustomAttribute(method, typeof(TAttribute), false);
                        if (attribute != null)
                        {
                            var data = new Pair<MethodInfo, TAttribute>(method, (TAttribute) attribute);
                            ReflectionCache.Instance[key] = data;
                            return data;
                        }
                    }
                }
                return null;
            }
            return (Pair<MethodInfo, TAttribute>) cached;
        }

        public static TAttribute RetrieveMethodAttributeOnly<TAttribute>(object target)
                where TAttribute : Attribute
        {
            var key = new Pair<Type, object>(typeof(TAttribute), target);
            var cached = ReflectionCache.Instance[key];
            if (cached == null)
            {
                var info = target.GetType().GetMethods();
                foreach (MethodInfo method in info)
                {
                    if (!method.IsStatic)
                    {
                        Attribute attribute =
                            Attribute.GetCustomAttribute(method, typeof(TAttribute), false);
                        if (attribute != null)
                        {
                            var data = (TAttribute) attribute;
                            ReflectionCache.Instance[key] = data;
                            return data;
                        }
                    }
                }
                return null;
            }
            return (TAttribute) cached;
        }

        public static IList<TAttribute> RetrievePropertyAttributeList<TAttribute>(object target)
                where TAttribute : Attribute
        {
            var key = new Pair<Type, object>(typeof(IList<TAttribute>), target);
            var cached = ReflectionCache.Instance[key];
            if (cached == null)
            {
                IList<TAttribute> list = new List<TAttribute>();
                var info = target.GetType().GetProperties();

                foreach (var property in info)
                {
                    if (property != null && (property.CanRead && property.CanWrite))
                    {
                        var setMethod = property.GetSetMethod();
                        if (setMethod != null && !setMethod.IsStatic)
                        {
                            var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                            if (attribute != null)
                            {
                                list.Add((TAttribute) attribute);
                            }
                        }
                    }
                }
                ReflectionCache.Instance[key] = list;
                return list;
            }
            return (IList<TAttribute>) cached;
        }

        public static TAttribute GetAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            object[] a = AssemblyFromWhichToPullInformation.GetCustomAttributes(typeof(TAttribute), false);
            if (a.Length <= 0) { return null; }

            return (TAttribute)a[0];
        }

        /// <summary>
        /// Setter provided for testing purpose.
        /// </summary>
        public static Assembly AssemblyFromWhichToPullInformation { get; set; }

        public static Pair<PropertyInfo, TAttribute> RetrieveOptionProperty<TAttribute>(object target, string uniqueName)
                where TAttribute : BaseOptionAttribute
        {
            var key = new Pair<Type, object>(typeof(Pair<PropertyInfo, BaseOptionAttribute>), target);
            var cached = ReflectionCache.Instance[key];
            if (cached == null)
            {
                Pair<PropertyInfo, TAttribute> found = null;
                if (target == null)
                {
                    return null;
                }
                var propertiesInfo = target.GetType().GetProperties();

                foreach (var property in propertiesInfo)
                {
                    if (property != null && (property.CanRead && property.CanWrite))
                    {
                        var setMethod = property.GetSetMethod();
                        if (setMethod != null && !setMethod.IsStatic)
                        {
                            var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                            var optionAttr = (TAttribute) attribute;
                            if (optionAttr != null && string.CompareOrdinal(uniqueName, optionAttr.UniqueName) == 0)
                            {
                                found = new Pair<PropertyInfo, TAttribute>(property, (TAttribute) attribute);
                                ReflectionCache.Instance[key] = found;
                                return found;
                            }
                        }
                    }
                }
            }
            return (Pair<PropertyInfo, TAttribute>) cached;
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}