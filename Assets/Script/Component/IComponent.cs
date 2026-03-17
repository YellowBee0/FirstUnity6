namespace YBFramework.Component
{
    public interface IComponent
    {
        /// <summary>获取存放组件的Entity</summary>
        /// <returns>Entity</returns>
        Entity GetOwner();

        /// <summary>初始化组件，比如在这一步获取其他组件</summary>
        /// <param name="entity">组件存放的Entity容器</param>
        void OnAddComponent(Entity entity);

        /// <summary>这一步相当于销毁组件，但是不会涉及到Entity销毁，除非是Entity销毁时调用，这里需要释放资源或者组件归还对象池（如果存在）</summary>
        void OnRemoveComponent();

        /// <summary>这一步用于重置组件状态，这里应该是Entity归还对象池而不是组件归还对象池，这一步不会释放任何资源，而是重置</summary>
        void ResetComponent();
    }
}