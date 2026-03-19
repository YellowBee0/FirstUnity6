using System;
using UnityEngine;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("修改层数")]
#endif
    [Serializable]
    public sealed class AddLayerProcess : RepeatAddProcess
    {
        [SerializeField] private int m_ModifyLayerCount;

        public override bool DoRepeatAdd(Buff existBuff)
        {
            //在这获取添加层数的行为
            //把m_ModifyLayerCount修改进去
            return false;
        }
    }
}