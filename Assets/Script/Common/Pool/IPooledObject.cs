namespace YBFramework.Common
{
    public interface IPooledObject
    {
        /// <summary>
        /// 归还对象池时释放使用的资源
        /// </summary>
        void OnFree();
    }
}