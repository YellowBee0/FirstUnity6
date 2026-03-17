using System;
using System.Reflection;

namespace YBFramework.Component
{
    [Serializable]
    public sealed class MethodPort : BasePort
    {
        private MethodInfo m_MethodInfo;

        public MethodPort(MethodInfo methodInfo)
        {
            m_MethodInfo = methodInfo;
#if UNITY_EDITOR
            m_ParameterInfos = methodInfo.GetParameters();
            m_ReturnType = m_MethodInfo.ReturnType;
#endif
        }

        public override void Copy(BasePort from)
        {
            base.Copy(from);
            if (from is MethodPort methodPort)
            {
                m_MethodInfo = methodPort.m_MethodInfo;
            }
        }

        public MethodInfo GetMethodInfo()
        {
            return m_MethodInfo;
        }
#if UNITY_EDITOR
        private ParameterInfo[] m_ParameterInfos;

        private Type m_ReturnType;

        public ParameterInfo[] GetParameters()
        {
            return m_ParameterInfos;
        }

        public Type GetReturnType()
        {
            return m_ReturnType;
        }

        public override string GetConnectTip()
        {
            s_StrBuilder.Clear();
            if (m_ParameterInfos.Length > 0)
            {
                s_StrBuilder.Append("(");
                for (int i = 0; i < m_ParameterInfos.Length; i++)
                {
                    s_StrBuilder.Append(m_ParameterInfos[i].ParameterType);
                }
                s_StrBuilder.Append(") : ");
            }
            s_StrBuilder.Append(m_ReturnType);
            return s_StrBuilder.ToString();
        }
#endif
    }
}