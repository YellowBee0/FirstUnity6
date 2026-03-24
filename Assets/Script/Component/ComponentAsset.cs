using System.Collections.Generic;
using UnityEngine;

namespace YBFramework.Component
{
    [CreateAssetMenu(menuName = "YBFramework/Component/Create Component")]
    public sealed class ComponentAsset : ScriptableObject
    {
        //TODO:对于需要继承MonoBehaviour的脚本，需要实现一个IComponent组件和一个MonoBehaviour组件。
        //TODO:这个IComponent组件只保存数据，在Clone的时候添加MonoBehaviour组件到对应的GameObject上，MonoBehaviour组件负责执行逻辑，需要使用IComponent组件保存的数据
        [SerializeReference] private List<IComponent> m_Components;

        public IReadOnlyList<IComponent> GetComponents()
        {
            return m_Components;
        }
    }
}