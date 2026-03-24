using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace YBFramework.Component
{
    public static class PortConnectHelper
    {
        private static readonly List<WrapMethodInfo> s_WrapMethodInfos = new();
        
        [RuntimeInitializeOnLoadMethod]
        public static void RegisterWrapMethod()
        {
            MethodInfo[] methodInfos = typeof(MethodWrapper).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            for (int i = 0; i < methodInfos.Length; i++)
            {
                s_WrapMethodInfos.Add(new WrapMethodInfo(methodInfos[i]));
            }
        }

        public static Delegate WrapMethod(Type delegateType, MethodInfo methodInfo, object target)
        {
            for (int i = 0; i < s_WrapMethodInfos.Count; i++)
            {
                MethodInfo wrapMethodInfo = s_WrapMethodInfos[i].GetMethodInfo(delegateType);
                if (wrapMethodInfo != null)
                {
                    MethodWrapper methodWrapper = new(methodInfo, target, s_WrapMethodInfos[i].GetParameterCount());
                    return wrapMethodInfo.CreateDelegate(delegateType, methodWrapper);
                }
            }
            Debug.LogError("Missing wrapper function for the delegate type");
            return null;
        }

        private sealed class MethodWrapper
        {
            private readonly MethodInfo m_MethodInfo;

            private readonly object[] m_Parameters;

            private readonly object m_Target;

            public MethodWrapper(MethodInfo methodInfo, object target, int parameterCount)
            {
                m_MethodInfo = methodInfo;
                m_Target = target;
                if (parameterCount > 0)
                {
                    m_Parameters = new object[parameterCount];
                }
            }

            public object NullObject()
            {
                return m_MethodInfo.Invoke(m_Target, m_Parameters);
            }

            public void NullVoid()
            {
                m_MethodInfo.Invoke(m_Target, m_Parameters);
            }

            public void IntVoid(int intValue)
            {
                m_Parameters[0] = intValue;
                m_MethodInfo.Invoke(m_Target, m_Parameters);
            }

            public void IntFloatVoid(int intValue, float floatValue)
            {
                m_Parameters[0] = intValue;
                m_Parameters[1] = floatValue;
                m_MethodInfo.Invoke(m_Target, m_Parameters);
            }
        }

        private sealed class WrapMethodInfo
        {
            private readonly HashSet<Type> m_AllowDelegateTypes;

            private readonly MethodInfo m_MethodInfo;

            private readonly ParameterInfo[] m_Parameters;

            private readonly Type m_ReturnType;

            public WrapMethodInfo(MethodInfo methodInfo)
            {
                m_AllowDelegateTypes = new HashSet<Type>();
                m_MethodInfo = methodInfo;
                if (methodInfo != null)
                {
                    m_Parameters = methodInfo.GetParameters();
                    m_ReturnType = methodInfo.ReturnType;
                }
            }

            private bool RegisterDelegate(Type delegateType)
            {
                if (m_AllowDelegateTypes.Contains(delegateType))
                {
                    return false;
                }
                MethodInfo methodInfo = delegateType.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
                if (methodInfo != null)
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    Type returnType = methodInfo.ReturnType;
                    int length = m_Parameters.Length;
                    if (length != parameters.Length)
                    {
                        return false;
                    }
                    for (int i = 0; i < length; i++)
                    {
                        ParameterInfo left = m_Parameters[i];
                        ParameterInfo right = parameters[i];
                        if (left.ParameterType.IsByRef == right.ParameterType.IsByRef)
                        {
                            if (left.IsIn != right.IsIn)
                            {
                                return false;
                            }
                            if (left.IsOut != right.IsOut)
                            {
                                return false;
                            }
                        }
                        if (left.ParameterType != right.ParameterType)
                        {
                            return false;
                        }
                    }
                    if (m_ReturnType != returnType)
                    {
                        return false;
                    }
                }
                m_AllowDelegateTypes.Add(delegateType);
                return true;
            }

            public MethodInfo GetMethodInfo(Type delegateType)
            {
                if (m_AllowDelegateTypes.Contains(delegateType) || RegisterDelegate(delegateType))
                {
                    return m_MethodInfo;
                }
                return null;
            }

            public int GetParameterCount()
            {
                return m_Parameters.Length;
            }
        }
    }
}