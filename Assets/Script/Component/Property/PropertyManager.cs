using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("属性")]
#endif
    [Serializable]
    public sealed class PropertyManager : IComponent
    {
        [SerializeField] private List<Property> m_Properties;

        private Entity m_Owner;

        public Entity GetOwner()
        {
            return m_Owner;
        }

        public void OnAddComponent(Entity entity)
        {
            m_Owner = entity;
        }

        public void OnRemoveComponent()
        {
            ObjectPool.Free(m_Properties);
            m_Properties.Clear();
        }

        public void ResetComponent()
        {
        }

        public IComponent Clone()
        {
            PropertyManager manager = new();
            int count = m_Properties.Count;
            Property[] properties = ObjectPool.Allocate<Property>(count);
            manager.m_Properties = new List<Property>(count);
            for (int i = 0; i < count; i++)
            {
                Property property = properties[i];
                property.CopyFrom(m_Properties[i]);
                manager.m_Properties.Add(property);
            }
            ArrayPool<Property>.Shared.Return(properties);
            return manager;
        }

        public Property GetProperty(PropertyType propertyType)
        {
            for (int i = 0; i < m_Properties.Count; i++)
            {
                if (m_Properties[i].GetPropertyType() == propertyType)
                {
                    return m_Properties[i];
                }
            }
            return null;
        }

        public void AddProperty(Property property)
        {
            if (GetProperty(property.GetPropertyType()) == null)
            {
                m_Properties.Add(property);
            }
        }

        public void RemoveProperty(PropertyType propertyType)
        {
            for (int i = 0; i < m_Properties.Count; i++)
            {
                Property property = m_Properties[i];
                if (property.GetPropertyType() == propertyType)
                {
                    ObjectPool.Free(property);
                    m_Properties.RemoveAt(i);
                    return;
                }
            }
        }
    }
}