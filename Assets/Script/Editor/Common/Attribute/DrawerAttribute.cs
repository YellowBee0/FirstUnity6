using System;

namespace YBFramework.MyEditor.Common
{
    //TODO:这个同样只服务于Graph或者不需要这个类型
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