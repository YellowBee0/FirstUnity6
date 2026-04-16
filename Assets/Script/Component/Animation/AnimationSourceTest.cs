using System;
using UnityEngine;
using YBFramework.MyEditor;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [AnimationEventSource("Test")]
#endif
    public sealed class AnimationSourceTest : IAnimationEventSource
    {
        private readonly int m_IntMember;

        public string GetSourceName()
        {
            return "Test";
        }

        private void HelloWorld()
        {
            Debug.LogError($"Hello World! {m_IntMember}");
        }

        private void TestFunction1(int intArg, string stringArg)
        {
            Debug.LogError($"intArg {intArg}; stringArg {stringArg} {m_IntMember}");
        }

        private void TestFunction2(int oneArg)
        {
            Debug.LogError($"single arg {oneArg} {m_IntMember}");
        }

#if UNITY_EDITOR
        [AnimationEvent("输出HelloWorld", 0)]
#endif
        private void EventFunction1(AnimationEventArgs _)
        {
            HelloWorld();
        }

#if UNITY_EDITOR
        [AnimationEvent("int参数和string参数的事件", 1)]
#endif
        private void EventFunction2(IntStringArg arg)
        {
            TestFunction1(arg.IntArg, arg.StringArg);
        }

#if UNITY_EDITOR
        [AnimationEvent("一个参数的事件", 2)]
#endif
        private void EventFunction3(IntArg arg)
        {
            TestFunction2(arg.Arg);
        }

        public AnimationEvent CreateAnimationEvent(int index, AnimationEventData eventData)
        {
            switch (index)
            {
                case 0:
                    return new AnimationEvent<AnimationEventArgs>(this, eventData, EventFunction1);
                case 1:
                    return new AnimationEvent<IntStringArg>(this, eventData, EventFunction2);
                case 2:
                    return new AnimationEvent<IntArg>(this, eventData, EventFunction3);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}