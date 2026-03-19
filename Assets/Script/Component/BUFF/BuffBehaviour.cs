using System;
using System.Collections.Generic;

namespace YBFramework.Component
{
    [Serializable]
    public abstract class BuffBehaviour
    {
        protected Buff m_BUFF;

        public abstract void OnMagnificationChanged();

        public abstract BuffBehaviour Clone();

        public abstract void OnInit(BuffBehaviour source);

        public abstract void OnReset();

        public virtual void OnAdd(Buff buff)
        {
            m_BUFF = buff;
        }

        public virtual void OnRemove()
        {
            Free(this);
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnStop()
        {
        }

        #region Pool

        private static readonly Dictionary<Type, Queue<BuffBehaviour>> s_BehaviourPools = new();

        private static Queue<BuffBehaviour> GetPool(Type componentType)
        {
            if (!s_BehaviourPools.TryGetValue(componentType, out var pool))
            {
                pool = new Queue<BuffBehaviour>();
                s_BehaviourPools.Add(componentType, pool);
            }
            return pool;
        }

        protected static BuffBehaviour Allocate<T>() where T : BuffBehaviour, new()
        {
            Queue<BuffBehaviour> pool = GetPool(typeof(T));
            return pool.Count > 0 ? pool.Dequeue() : new T();
        }

        private static void Free(BuffBehaviour component)
        {
            GetPool(component.GetType()).Enqueue(component);
        }

        #endregion
    }
}