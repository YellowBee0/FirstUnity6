using System;
using System.Collections.Generic;
using UnityEngine;

namespace YBFramework.MyEditor
{
    public static class DerivedTypeManager
    {
        private static readonly Dictionary<Type, ValueTuple<List<string>, List<Type>>> s_Groups = new();

        private static string DefaultNameSelector(Type type)
        {
            return type.Name;
        }

        public static void RegisterDerivedTypes(Type baseType, IEnumerable<Type> derivedTypes, Func<Type, string> nameSelector)
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
            if (s_Groups.TryGetValue(baseType, out ValueTuple<List<string>, List<Type>> group))
            {
                Debug.LogWarning($"{baseType} has been registered");
            }
            group = new ValueTuple<List<string>, List<Type>>();
            foreach (Type type in derivedTypes)
            {
                if (!type.IsAbstract && !type.IsGenericTypeDefinition)
                {
                    group.Item1.Add(nameSelector(type));
                    group.Item2.Add(type);
                }
            }
            s_Groups.Add(baseType, group);
        }

        public static void RegisterDerivedType(Type baseType, Type derivedType, string typeName)
        {
            if (derivedType == null || derivedType == null)
            {
                throw new ArgumentNullException();
            }
            if (!derivedType.IsAbstract && !derivedType.IsGenericTypeDefinition && baseType.IsAssignableFrom(derivedType))
            {
                if (!s_Groups.TryGetValue(baseType, out ValueTuple<List<string>, List<Type>> group))
                {
                    group = new ValueTuple<List<string>, List<Type>>();
                    s_Groups.Add(baseType, group);
                }
                if (group.Item2.Contains(derivedType))
                {
                    Debug.LogWarning($"{derivedType} has been added");
                }
                group.Item1.Add(string.IsNullOrEmpty(typeName) ? DefaultNameSelector(derivedType) : typeName);
                group.Item2.Add(derivedType);
            }
        }

        public static ValueTuple<List<string>, List<Type>> GetDerivedTypes(Type baseType)
        {
            return s_Groups.GetValueOrDefault(baseType);
        }
    }
}