#if UNITY_EDITOR
using System;

namespace YBFramework.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DrawTargetAttribute : Attribute
    {
    }
}
#endif