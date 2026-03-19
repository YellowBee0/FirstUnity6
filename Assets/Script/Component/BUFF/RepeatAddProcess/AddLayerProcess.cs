using System;
using UnityEngine;

namespace YBFramework.Component
{
    [Serializable]
    public sealed class AddLayerProcess : RepeatAddProcess
    {
        [SerializeField]
        private int m_ModifyLayerCount;

        public override bool DoRepeatAdd(Buff existBuff)
        {
            //在这获取添加层数的行为
            //把m_ModifyLayerCount修改进去
            return false;
        }
    }
}