using System;

namespace YBFramework.Component
{
    public interface IExecutor
    {
        void Initialize(Entity entity);
        
        void Start();

        void Stop();

        void Reset();

        void RegisterExecuteCallback(Action callback);
        
        void UnregisterExecuteCallback(Action callback);

        IExecutor Clone();
    }
}