using System;
using System.Collections.Generic;

namespace YBFramework.Component
{
    public interface IBuffComponent
    {
        private static readonly Dictionary<Type, Queue<IBuffComponent>> s_ComponentPools = new();

        private static Queue<IBuffComponent> GetPool(Type componentType)
        {
            if (!s_ComponentPools.TryGetValue(componentType, out Queue<IBuffComponent> pool))
            {
                pool = new Queue<IBuffComponent>();
                s_ComponentPools.Add(componentType, pool);
            }
            return pool;
        }

        protected static T Allocate<T>() where T : IBuffComponent, new()
        {
            Queue<IBuffComponent> pool = GetPool(typeof(T));
            return pool.Count > 0 ? (T)pool.Dequeue() : new T();
        }

        protected static void Free(IBuffComponent component)
        {
            GetPool(component.GetType()).Enqueue(component);
        }

        void OnAdd(Buff buff);

        void OnRemove();

        void OnReset();

        void OnMagnificationChanged();

        IBuffComponent Clone();
    }
}