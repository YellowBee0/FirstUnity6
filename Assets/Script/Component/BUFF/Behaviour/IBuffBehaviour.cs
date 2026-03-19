using System;
using System.Collections.Generic;

namespace YBFramework.Component
{
    public interface IBuffBehaviour
    {
        private static readonly Dictionary<Type, Queue<IBuffBehaviour>> s_BehaviourPools = new();

        private static Queue<IBuffBehaviour> GetPool(Type componentType)
        {
            if (!s_BehaviourPools.TryGetValue(componentType, out Queue<IBuffBehaviour> pool))
            {
                pool = new Queue<IBuffBehaviour>();
                s_BehaviourPools.Add(componentType, pool);
            }
            return pool;
        }

        protected static T Allocate<T>() where T : IBuffBehaviour, new()
        {
            Queue<IBuffBehaviour> pool = GetPool(typeof(T));
            return pool.Count > 0 ? (T)pool.Dequeue() : new T();
        }

        protected static void Free(IBuffBehaviour behaviour)
        {
            GetPool(behaviour.GetType()).Enqueue(behaviour);
        }

        void OnAdd(Buff buff);

        void OnRemove();

        void OnReset();

        void OnMagnificationChanged();

        IBuffBehaviour Clone();
    }
}