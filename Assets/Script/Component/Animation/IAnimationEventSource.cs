namespace YBFramework.Component
{
    public interface IAnimationEventSource
    {
        string GetSourceName();

        AnimationEvent CreateAnimationEvent(int index, AnimationEventData eventData);
    }
}