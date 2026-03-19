using System;

namespace YBFramework.Component
{
    public interface IExecutor
    {
        void Initialize(Entity entity);
        
        void Start();

        void Stop();

        void Reset();

        void SetExecuteAction(Action action);

        IExecutor Clone();
    }
}