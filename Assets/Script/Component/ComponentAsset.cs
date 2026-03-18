using System.Collections.Generic;
using UnityEngine;

namespace YBFramework.Component
{
    [CreateAssetMenu(menuName = "YBFramework/Component/Create Component")]
    public sealed class ComponentAsset : ScriptableObject
    {
        [SerializeReference] private List<IComponent> m_Components;
    }
}