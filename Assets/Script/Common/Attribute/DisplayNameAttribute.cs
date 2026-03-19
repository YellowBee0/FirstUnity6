using System;

namespace YBFramework.Common
{
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class DisplayNameAttribute : Attribute
    {
        public readonly string Name;

        public DisplayNameAttribute(string name)
        {
            Name = name;
        }
    }
}