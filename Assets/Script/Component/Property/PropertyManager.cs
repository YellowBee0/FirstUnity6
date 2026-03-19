using System.Collections.Generic;

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
            Property.Free(m_Properties.Values);
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
            if (m_Properties.ContainsKey(name))
            {
                m_Properties.Add(name, property);
            }
        }

        public void RemoveProperty(string name)
        {
            if (m_Properties.Remove(name, out Property property))
            {
                Property.Free(property);
            }
        }
    }
}