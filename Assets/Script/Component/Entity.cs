using System;
using System.Collections.Generic;
using UnityEngine;

namespace YBFramework.Component
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private ComponentAsset m_ComponentAsset;

        private Dictionary<Type, IComponent> m_Components;

        public void Awake()
        {
            IReadOnlyList<IComponent> components = m_ComponentAsset.GetComponents();
            int count = components.Count;
            m_Components = new Dictionary<Type, IComponent>(count);
            for (int i = 0; i < count; i++)
            {
                IComponent component = components[i];
                m_Components.Add(component.GetType(), component.Clone());
            }
            foreach (KeyValuePair<Type, IComponent> kvp in m_Components)
            {
                kvp.Value.OnAddComponent(this);
            }
        }

        public void AddComponent(IComponent component)
        {
            if (m_Components.TryAdd(component.GetType(), component))
            {
                component.OnAddComponent(this);
            }
        }

        public void RemoveComponent(IComponent component)
        {
            if (m_Components.Remove(component.GetType()))
            {
                component.OnRemoveComponent();
            }
        }

        public T GetCustomComponent<T>() where T : class, IComponent
        {
            return m_Components.TryGetValue(typeof(T), out IComponent component) ? (T)component : null;
        }

        /// <summary>可以在归还对象池的时候调用这个函数</summary>
        public void ResetEntity()
        {
            foreach (IComponent component in m_Components.Values)
            {
                component.ResetComponent();
            }
        }

        /// <summary>销毁Entity容器的时候会释放组件使用的资源，也有可能把组件归还到组件对象池（如果存在）</summary>
        private void OnDestroy()
        {
            foreach (KeyValuePair<Type, IComponent> kvp in m_Components)
            {
                kvp.Value.OnRemoveComponent();
            }
            m_Components.Clear();
        }
    }
}