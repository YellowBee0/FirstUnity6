using System.Reflection;

namespace YBFramework.Component
{
    public interface IAnimationEventSource
    {
        (string eventName, MethodInfo methodInfo)[] GetAnimationEventMethodInfos();
    }
}