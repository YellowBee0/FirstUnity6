using System.Collections.Generic;
using YBFramework.Common;

namespace YBFramework.Component
{
    public sealed class PropertyManager : IComponent
    {
        private readonly Dictionary<string, Property> m_Properties = new();

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
            ObjectPool.Free(m_Properties.Values);
            m_Properties.Clear();
        }

        public void ResetComponent()
        {
        }

        public Property GetProperty(string name)
        {
            return m_Properties.GetValueOrDefault(name);
        }

        public void AddProperty(string name, Property property)
        {
            m_Properties.TryAdd(name, property);
        }

        public void RemoveProperty(string name)
        {
            if (m_Properties.Remove(name, out Property property))
            {
                ObjectPool.Free(property);
            }
        }
    }
}