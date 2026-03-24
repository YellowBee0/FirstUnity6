using System;
using System.Buffers;
using System.Collections.Generic;

namespace YBFramework.Common
{
    public static class ObjectPool
    {
        private static readonly Dictionary<Type, Queue<IPooledObject>> s_Pools = new();

        private static Queue<IPooledObject> GetPool(Type type)
        {
            if (!s_Pools.TryGetValue(type, out Queue<IPooledObject> pool))
            {
                pool = new Queue<IPooledObject>();
                s_Pools.Add(type, pool);
            }
            return pool;
        }

        public static T Allocate<T>() where T : class, IPooledObject, new()
        {
            Queue<IPooledObject> pool = GetPool(typeof(T));
            return pool.Count > 0 ? (T)pool.Dequeue() : new T();
        }

        /// <summary>
        /// 使用ArrayPool分配数组集合，赋值操作完后需要归还数组
        /// </summary>
        /// <param name="count">希望分配数组大小，但是大小不一定是count</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>对象数组</returns>
        public static T[] Allocate<T>(int count) where T : class, IPooledObject, new()
        {
            Queue<IPooledObject> pool = GetPool(typeof(T));
            T[] objs = ArrayPool<T>.Shared.Rent(count);
            for (int i = 0; i < count; i++)
            {
                objs[i] = pool.Count > 0 ? (T)pool.Dequeue() : new T();
            }
            return objs;
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
    }
}