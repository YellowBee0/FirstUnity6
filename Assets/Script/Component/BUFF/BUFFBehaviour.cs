using System;
using System.Collections.Generic;

namespace YBFramework.Component
{
    [Serializable]
    public abstract class BUFFBehaviour
    {
        protected BUFF m_BUFF;

        public abstract void OnMagnificationChanged();

        public abstract BUFFBehaviour Clone();

        public abstract void OnInit(BUFFBehaviour source);

        public abstract void OnReset();

        public virtual void OnAdd(BUFF buff)
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

        private static readonly Dictionary<Type, Queue<BUFFBehaviour>> s_BehaviourPools = new();

        private static Queue<BUFFBehaviour> GetPool(Type componentType)
        {
            if (!s_BehaviourPools.TryGetValue(componentType, out var pool))
            {
                pool = new Queue<BUFFBehaviour>();
                s_BehaviourPools.Add(componentType, pool);
            }
            return pool;
        }

        protected static BUFFBehaviour Allocate<T>() where T : BUFFBehaviour, new()
        {
            Queue<BUFFBehaviour> pool = GetPool(typeof(T));
            return pool.Count > 0 ? pool.Dequeue() : new T();
        }

        private static void Free(BUFFBehaviour component)
        {
            GetPool(component.GetType()).Enqueue(component);
        }

        #endregion
    }
}