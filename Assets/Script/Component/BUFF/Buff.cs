using System;
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
            for (int i = 0; i < components.Count; i++)
            {
                AddComponent(components[i].Clone());
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

        public IBuffComponent GetComponent(Type type)
        {
            /*for (int i = 0; i < m_Behaviours.Count; i++)
            {
                if (m_Behaviours[i].GetType() == type)
                {
                    return m_Behaviours[i];
                }
            }*/
            return null;
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