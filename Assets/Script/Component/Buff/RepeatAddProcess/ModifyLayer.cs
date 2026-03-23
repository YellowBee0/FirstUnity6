using System;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("修改层数")]
#endif
    [Serializable]
    public sealed class ModifyLayer : RepeatAddProcess
    {
        [SerializeField] private int m_ModifyLayerCount;

        public override bool DoRepeatAdd(Buff existBuff)
        {
            existBuff.GetComponent<BuffLayer>()?.ModifyCurValue("Modify layer process", m_ModifyLayerCount);
            return false;
        }
    }
}