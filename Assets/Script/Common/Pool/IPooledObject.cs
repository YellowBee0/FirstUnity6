namespace YBFramework.Common
{
    public interface IPooledObject
    {
        void OnFree();
        
        void OnClear();
    }
}