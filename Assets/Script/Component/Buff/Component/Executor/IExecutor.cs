using System;
using YBFramework.Common;

namespace YBFramework.Component
{
    public interface IExecutor : IPooledObject
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