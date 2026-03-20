using System;
using System.Collections.Generic;

namespace YBFramework.Common
{
    public static class ObjectPool
    {
        private static readonly Dictionary<Type, Queue<IPooledObject>> s_Pools = new();

        public static T Allocate<T>() where T : class, IPooledObject, new()
        {
            Type type = typeof(T);
            if (!s_Pools.TryGetValue(type, out Queue<IPooledObject> pool))
            {
                pool = new Queue<IPooledObject>();
                s_Pools.Add(type, pool);
            }
            return pool.Count > 0 ? (T)pool.Dequeue() : new T();
        }

        public static void Free(IPooledObject obj)
        {
            Type type = obj.GetType();
            if (s_Pools.TryGetValue(type, out Queue<IPooledObject> pool))
            {
                obj.OnFree();
                pool.Enqueue(obj);
            }
        }

        public static void Free(Type type, IEnumerable<IPooledObject> objs)
        {
            if (s_Pools.TryGetValue(type, out Queue<IPooledObject> pool))
            {
                foreach (IPooledObject obj in objs)
                {
                    obj.OnFree();
                    pool.Enqueue(obj);
                }
            }
        }


        public static void Free<T>(IEnumerable<T> objs) where T : class, IPooledObject
        {
            Free(typeof(T), objs);
        }

        public static void Clear(Type type)
        {
            if (s_Pools.Remove(type, out Queue<IPooledObject> pool))
            {
                foreach (IPooledObject obj in pool)
                {
                    obj.OnClear();
                }
            }
        }

        public static void Clear<T>() where T : class, IPooledObject
        {
            Clear(typeof(T));
        }

        public static void ClearAll()
        {
            foreach (KeyValuePair<Type, Queue<IPooledObject>> kvp in s_Pools)
            {
                foreach (IPooledObject obj in kvp.Value)
                {
                    obj.OnClear();
                }
            }
            s_Pools.Clear();
        }
    }
}