using System;

namespace YBFramework.MyEditor.Common
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DrawerAttribute : Attribute
    {
        public readonly Type TargetType;

        public DrawerAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}