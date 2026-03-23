using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YBFramework.Component
{
    public sealed class Buff
    {
        private static readonly Queue<Buff> s_Pool = new();

        public static Buff Allocate(Entity caster, BuffAsset buffAsset, BuffManager manager)
        {
            Buff buff = s_Pool.Count > 0 ? s_Pool.Dequeue() : new Buff();
            buff.OnAllocate(caster, buffAsset, manager);
            return buff;
        }

        public static void Free(Buff buff)
        {
            buff.OnFree();
            s_Pool.Enqueue(buff);
        }

        private readonly List<IBuffComponent> m_Components = new();

        private Entity m_Caster;

        private BuffAsset m_BuffAsset;

        private BuffManager m_Manager;

        private float m_Magnification;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity GetCaster()
        {
            return m_Caster;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BuffAsset GetBuffAsset()
        {
            return m_BuffAsset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BuffManager GetManager()
        {
            return m_Manager;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetMagnification()
        {
            return m_Magnification;
        }

        private void OnAllocate(Entity caster, BuffAsset buffAsset, BuffManager manager)
        {
            m_Caster = caster;
            m_BuffAsset = buffAsset;
            m_Manager = manager;
            IReadOnlyList<IBuffComponent> components = buffAsset.GetComponents();
            //拆分为两个for循环是为了保证组件在OnAdd时能够获取其他组件
            for (int i = 0; i < components.Count; i++)
            {
                m_Components.Add(components[i].Clone());
            }
            for (int i = 0; i < m_Components.Count; i++)
            {
                m_Components[i].OnAdd(this);
            }
        }

        private void OnFree()
        {
            for (int i = 0; i < m_Components.Count; i++)
            {
                m_Components[i].OnRemove();
            }
            m_Components.Clear();
        }

        public T GetComponent<T>() where T : IBuffComponent
        {
            Type type = typeof(T);
            for (int i = 0; i < m_Components.Count; i++)
            {
                if (m_Components[i].GetType() == type)
                {
                    return (T)m_Components[i];
                }
            }
            return default;
        }

        /// <summary>
        /// 查找Buff中所有同类型的组件，数组通过ArrayPool创建，外部不再使用时需要使用ArrayPool.Shared.Return归还
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>数组集合</returns>
        public T[] GetComponents<T>() where T : IBuffComponent
        {
            Type type = typeof(T);
            T[] components = ArrayPool<T>.Shared.Rent(m_Components.Count);
            int index = 0;
            for (int i = 0; i < m_Components.Count; i++)
            {
                if (m_Components[i].GetType() == type)
                {
                    components[index++] = (T)m_Components[i];
                }
            }
            return components;
        }

        public void AddComponent(IBuffComponent component)
        {
            m_Components.Add(component);
            component.OnAdd(this);
        }

        public void RemoveComponent(IBuffComponent component)
        {
            if (m_Components.Remove(component))
            {
                component.OnRemove();
            }
        }

        public void Reset()
        {
            for (int i = 0; i < m_Components.Count; i++)
            {
                m_Components[i].OnReset();
            }
        }

        public void SetMagnification(float magnification)
        {
            if (!Mathf.Approximately(m_Magnification, magnification))
            {
                m_Magnification = magnification;
                for (int i = 0; i < m_Components.Count; i++)
                {
                    m_Components[i].OnMagnificationChanged();
                }
            }
        }
    }
}