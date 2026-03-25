using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace YBFramework.MyEditor.Common
{
    /// <summary>
    /// TODO:这个类型需要改为只服务于Graph的绘制
    /// </summary>
    public static class DrawerManager
    {
        private static readonly Dictionary<Type, Type> s_CommonDrawers = new();

        private static readonly Dictionary<Type, Type> s_GenericDrawers = new();

        private static readonly Dictionary<Type, Type> s_ActualGenericDrawers = new();

        private static readonly Dictionary<Type, Queue<object>> s_DrawerPools = new();

        [InitializeOnLoadMethod]
        private static void Init()
        {
            UnityEditor.TypeCache.TypeCollection types = UnityEditor.TypeCache.GetTypesWithAttribute<DrawerAttribute>();
            for (int i = 0; i < types.Count; i++)
            {
                DrawerAttribute attribute = types[i].GetCustomAttribute<DrawerAttribute>();
                if (attribute != null)
                {
                    Dictionary<Type, Type> drawers = attribute.TargetType.IsGenericType ? s_GenericDrawers : s_CommonDrawers;
                    if (!drawers.TryAdd(attribute.TargetType, types[i]))
                    {
                        throw new ArgumentException($"{attribute.TargetType} is already exists");
                    }
                }
            }
        }

        public static TDrawer Allocate<TDrawer, TTarget>()
        {
            return (TDrawer)Allocate(typeof(TTarget));
        }

        public static object Allocate(Type type)
        {
            Type drawerType = null;
            while (type != null && type != typeof(object))
            {
                //如果支持绘制interface的话需要在这里找到类型实现的所有interface，并查找是否有对应的实现类型
                if (type.IsGenericType)
                {
                    if (s_GenericDrawers.TryGetValue(type.GetGenericTypeDefinition(), out Type genericPortDrawer))
                    {
                        if (!s_ActualGenericDrawers.TryGetValue(type, out drawerType))
                        {
                            drawerType = genericPortDrawer.MakeGenericType(type.GetGenericArguments());
                            s_ActualGenericDrawers.Add(type, drawerType);
                        }
                        break;
                    }
                }
                else
                {
                    if (s_CommonDrawers.TryGetValue(type, out drawerType))
                    {
                        break;
                    }
                }
                type = type.BaseType;
            }
            if (drawerType != null)
            {
                if (!s_DrawerPools.TryGetValue(drawerType, out Queue<object> pool))
                {
                    pool = new Queue<object>();
                    s_DrawerPools.Add(drawerType, pool);
                }
                return pool.Count > 0 ? pool.Dequeue() : Activator.CreateInstance(drawerType);
            }
            return null;
        }

        public static void Free(object drawer)
        {
            Type drawerType = drawer.GetType();
            if (drawerType.IsDefined(typeof(DrawerAttribute)))
            {
                if (!s_DrawerPools.TryGetValue(drawerType, out Queue<object> pool))
                {
                    pool = new Queue<object>();
                    s_DrawerPools.Add(drawerType, pool);
                }
                pool.Enqueue(drawer);
            }
        }
    }
}