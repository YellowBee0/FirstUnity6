using System;

namespace YBFramework.Component
{
    public interface IAnimationEventSource
    {
        string GetSourceName();
        
        Action<object[]> CreateAnimationEvent(string eventName);
    }
}