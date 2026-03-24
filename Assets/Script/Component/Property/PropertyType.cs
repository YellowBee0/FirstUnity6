using UnityEngine;

namespace YBFramework.Component
{
    public enum PropertyType
    {
        [InspectorName("生命值")]
        Heal,
        [InspectorName("攻击力")]
        Attack,
        [InspectorName("速度")]
        Speed,
        [InspectorName("防御")]
        Defense
    }
}