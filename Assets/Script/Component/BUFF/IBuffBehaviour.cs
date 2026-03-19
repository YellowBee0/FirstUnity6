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

        protected static IBuffBehaviour Allocate<T>() where T : IBuffBehaviour, new()
        {
            Queue<IBuffBehaviour> pool = GetPool(typeof(T));
            return pool.Count > 0 ? pool.Dequeue() : new T();
        }

        private static void Free(IBuffBehaviour behaviour)
        {
            GetPool(behaviour.GetType()).Enqueue(behaviour);
        }

        void OnReset();

        void OnAdd(Buff buff);

        void OnRemove();

        void OnStart();

        void OnStop();

        void OnMagnificationChanged();

        IBuffBehaviour Clone();
    }
}