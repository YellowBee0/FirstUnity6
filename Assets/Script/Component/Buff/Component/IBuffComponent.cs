namespace YBFramework.Component
{
    public interface IBuffComponent
    {
        void OnAdd(Buff buff);

        void OnRemove();

        void OnReset();

        void OnMagnificationChanged();

        IBuffComponent Clone();
    }
}