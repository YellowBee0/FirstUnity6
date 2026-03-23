using System;
using System.Collections.Generic;
using System.Reflection;
using YBFramework.Common;

namespace YBFramework.MyEditor.Common
{
    public static class TypeCache
    {
        private static readonly Dictionary<Type, ValueTuple<List<Type>, List<string>>> s_DerivedClasses = new();

        public static ValueTuple<List<Type>, List<string>> GetDerivedClasses(Type type)
        {
            if (!s_DerivedClasses.TryGetValue(type, out ValueTuple<List<Type>, List<string>> derivedClasses))
            {
                List<Type> derivedTypes = new();
                List<string> derivedDisplayNames = new();
                UnityEditor.TypeCache.TypeCollection types = UnityEditor.TypeCache.GetTypesDerivedFrom(type);
                foreach (Type derivedType in types)
                {
                    if (!derivedType.IsAbstract)
                    {
                        derivedTypes.Add(derivedType);
                        DisplayNameAttribute displayName = derivedType.GetCustomAttribute<DisplayNameAttribute>();
                        derivedDisplayNames.Add(displayName != null ? displayName.Name : derivedType.Name);
                    }
                }
                derivedDisplayNames.Add("空");
                derivedClasses = new ValueTuple<List<Type>, List<string>>(derivedTypes, derivedDisplayNames);
                s_DerivedClasses.Add(type, derivedClasses);
            }
            return derivedClasses;
        }

        public static Type GetDerivedClassType(Type type, string displayName)
        {
            int index = -1;
            (List<Type> derivedTypes, List<string> derivedDisplayNames) = GetDerivedClasses(type);
            for (int i = 0; i < derivedDisplayNames.Count; i++)
            {
                if (derivedDisplayNames[i] == displayName)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0 && index < derivedTypes.Count)
            {
                return derivedTypes[index];
            }
            return null;
        }

        public static string GetDerivedClassDisplayName(Type type, Type derivedType)
        {
            int index = -1;
            (List<Type> derivedTypes, List<string> derivedDisplayNames) = GetDerivedClasses(type);
            for (int i = 0; i < derivedTypes.Count; i++)
            {
                if (derivedTypes[i] == derivedType)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0 && index < derivedDisplayNames.Count)
            {
                return derivedDisplayNames[index];
            }
            //TODO:这里不能返回空，而是需要一个默认值，默认这个类型
            return "空";
        }
    }
}