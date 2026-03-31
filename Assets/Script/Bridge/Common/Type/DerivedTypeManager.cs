using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using YBFramework.Common;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public static class DerivedTypeManager
    {
        private static readonly Dictionary<Type, ValueTuple<List<string>, List<Type>>> s_Groups = new();

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            RegisterDerivedTypes(typeof(object), new[]
            {
                typeof(object),
                typeof(bool),
                typeof(int),
                typeof(float),
                typeof(Color),
                typeof(string)
            }, false, null);
            RegisterDerivedTypes(typeof(IBuffComponent), TypeCache.GetTypesDerivedFrom<IBuffComponent>(), true, GetDisplayName);
            RegisterDerivedTypes(typeof(IExecutor), TypeCache.GetTypesDerivedFrom<IExecutor>(), false, GetDisplayName);
            RegisterDerivedTypes(typeof(IComponent), TypeCache.GetTypesDerivedFrom<IComponent>(), true, GetDisplayName);
            RegisterDerivedTypes(typeof(RepeatAddProcess), TypeCache.GetTypesDerivedFrom<RepeatAddProcess>(), true, GetDisplayName);
        }

        private static string GetDisplayName(Type type)
        {
            DisplayNameAttribute displayNameAttribute = type.GetCustomAttribute<DisplayNameAttribute>();
            return displayNameAttribute != null ? displayNameAttribute.Name : type.Name;
        }

        private static string DefaultNameSelector(Type type)
        {
            return type.Name;
        }

        public static void RegisterDerivedTypes(Type baseType, IEnumerable<Type> derivedTypes, bool registerNull, Func<Type, string> nameSelector)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }
            if (derivedTypes == null)
            {
                throw new ArgumentNullException(nameof(derivedTypes));
            }
            nameSelector ??= DefaultNameSelector;
            if (!s_Groups.TryGetValue(baseType, out ValueTuple<List<string>, List<Type>> group))
            {
                List<string> derivedTypeNamesList = new();
                List<Type> derivedTypesList = new();
                group = new ValueTuple<List<string>, List<Type>>(derivedTypeNamesList, derivedTypesList);
                if (registerNull)
                {
                    derivedTypeNamesList.Add("Null");
                    derivedTypesList.Add(null);
                }
                foreach (Type type in derivedTypes)
                {
                    if (type != null && !type.IsAbstract && !type.IsGenericTypeDefinition)
                    {
                        derivedTypeNamesList.Add(nameSelector(type));
                        derivedTypesList.Add(type);
                    }
                }
                s_Groups.Add(baseType, group);
            }
        }

        public static ValueTuple<List<string>, List<Type>> GetDerivedTypes(Type baseType)
        {
            return s_Groups.GetValueOrDefault(baseType);
        }
    }
}