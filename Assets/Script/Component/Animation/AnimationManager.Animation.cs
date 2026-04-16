using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace YBFramework.Component
{
    public sealed partial class AnimationManager
    {
        private sealed class Animation
        {
            private static class AnimationEventInvoker
            {
                private sealed class InvokeState
                {
                    private static readonly Queue<InvokeState> s_InvokeStatePool = new();

                    public static InvokeState Allocate(AnimationClipPlayable animationClipPlayable, IReadOnlyList<AnimationEvent> animationEvents, float lenght, bool isLooping)
                    {
                        InvokeState state = s_InvokeStatePool.Count > 0 ? s_InvokeStatePool.Dequeue() : new InvokeState();
                        state.m_AnimationClipPlayable = animationClipPlayable;
                        state.m_AnimationEvents = animationEvents;
                        state.m_Lenght = lenght;
                        state.m_PreTime = 0;
                        state.m_InvokedEventCount = 0;
                        state.m_IsLooping = isLooping;
                        return state;
                    }

                    public static void Free(InvokeState state)
                    {
                        s_InvokeStatePool.Enqueue(state);
                    }

                    private AnimationClipPlayable m_AnimationClipPlayable;

                    private IReadOnlyList<AnimationEvent> m_AnimationEvents;

                    private float m_Lenght;

                    private float m_PreTime;

                    private int m_InvokedEventCount;

                    private bool m_IsLooping;

                    public void OnUpdate()
                    {
                        float curTime = (float)m_AnimationClipPlayable.GetTime() % m_Lenght;
                        //如果上一帧时间大于当前时间，说明动画已经循环了
                        if (m_PreTime > curTime)
                        {
                            //保证每个动画事件都会触发
                            for (int i = m_InvokedEventCount; i < m_AnimationEvents.Count; i++)
                            {
                                m_AnimationEvents[i].Invoke();
                            }
                            m_InvokedEventCount = m_IsLooping ? 0 : m_AnimationEvents.Count;
                        }
                        //从上次触发事件的索引开始，遍历事件列表，触发时间小于等于当前时间的事件
                        for (int i = m_InvokedEventCount; i < m_AnimationEvents.Count; i++)
                        {
                            if (m_AnimationEvents[i].GetEventData().TriggerTime <= curTime)
                            {
                                m_AnimationEvents[i].Invoke();
                                m_InvokedEventCount++;
                            }
                            //因为事件列表是有序的，所以如果当前事件的触发时间大于当前时间，则后续事件都不会触发
                            else
                            {
                                break;
                            }
                        }
                        //更新上一帧时间
                        m_PreTime = curTime;
                    }
                }

                private static readonly List<InvokeState> s_InvokeStates = new();

                private static bool s_IsUpdating;

                public static void AddEventListener(Animation animation)
                {
                    if (animation.m_AnimationEvents.Count > 0)
                    {
                        AnimationClip animationClip = animation.m_AnimationAsset.GetAnimationClip();
                        InvokeState invokeState = InvokeState.Allocate(animation.m_AnimationClipPlayable, animation.m_AnimationEvents, animationClip.length, animationClip.isLooping);
                        animation.m_InvokeStateIndex = s_InvokeStates.Count;
                        s_InvokeStates.Add(invokeState);
                        if (!s_IsUpdating)
                        {
                            Update().Forget();
                            s_IsUpdating = true;
                        }
                    }
                }

                /// <summary>
                ///     移除一个动画的事件监听者，这个函数不能在动画事件中调用，因为集合在轮询的过程中移除（除了移除最后一个元素的情况）都会导致索引略过某一个元素
                /// </summary>
                /// <param name="animation">动画</param>
                public static void RemoveEventListener(Animation animation)
                {
                    if (animation.m_InvokeStateIndex != -1)
                    {
                        int lastIndex = s_InvokeStates.Count - 1;
                        (s_InvokeStates[animation.m_InvokeStateIndex], s_InvokeStates[lastIndex]) = (s_InvokeStates[lastIndex], s_InvokeStates[animation.m_InvokeStateIndex]);
                        InvokeState.Free(s_InvokeStates[lastIndex]);
                        s_InvokeStates.RemoveAt(lastIndex);
                        if (lastIndex == 0 && s_IsUpdating)
                        {
                            s_IsUpdating = false;
                        }
                        animation.m_InvokeStateIndex = -1;
                    }
                }

                private static async UniTaskVoid Update()
                {
                    while (s_IsUpdating)
                    {
                        await UniTask.NextFrame();
                        for (int i = 0; i < s_InvokeStates.Count; i++)
                        {
                            s_InvokeStates[i].OnUpdate();
                        }
                    }
                }
            }

            private static readonly Queue<Animation> s_AnimationPool = new();

            public static Animation Allocate(AnimationAsset animationAsset, AnimationClipPlayable animationClipPlayable)
            {
                Animation animation = s_AnimationPool.Count > 0 ? s_AnimationPool.Dequeue() : new Animation();
                animation.m_AnimationAsset = animationAsset;
                animation.m_AnimationClipPlayable = animationClipPlayable;
                return animation;
            }

            public static void Free(Animation animation)
            {
                animation.RegisteredPort = null;
                animation.m_AnimationAsset = null;
                animation.m_AnimationClipPlayable = default;
                animation.m_AnimationEvents.Clear();
                animation.m_IsPlaying = false;
                s_AnimationPool.Enqueue(animation);
            }

            public AnimationPort RegisteredPort;

            private AnimationAsset m_AnimationAsset;

            private AnimationClipPlayable m_AnimationClipPlayable;

            private readonly List<AnimationEvent> m_AnimationEvents = new();

            private int m_InvokeStateIndex;

            private bool m_IsPlaying;

            public AnimationAsset GetAnimationAsset()
            {
                return m_AnimationAsset;
            }

            public AnimationClipPlayable GetAnimationClipPlayable()
            {
                return m_AnimationClipPlayable;
            }

            public void Play()
            {
                if (m_IsPlaying)
                {
                    return;
                }
                AnimationEventInvoker.AddEventListener(this);
                m_AnimationClipPlayable.Play();
                m_IsPlaying = true;
            }

            public void Pause()
            {
                if (m_IsPlaying)
                {
                    m_AnimationClipPlayable.Pause();
                    AnimationEventInvoker.RemoveEventListener(this);
                    m_IsPlaying = false;
                }
            }

            public void Reset()
            {
                m_AnimationClipPlayable.SetTime(0);
            }

            public void SetSpeed(float speed)
            {
                m_AnimationClipPlayable.SetSpeed(speed);
            }

            public void AddAnimationEvent(AnimationEventData eventData, IAnimationEventSource eventSource)
            {
                int eventIndex = eventData.EventIndex;
                float triggerTime = eventData.TriggerTime;
                for (int i = 0; i < m_AnimationEvents.Count; i++)
                {
                    AnimationEvent animationEvent = m_AnimationEvents[i];
                    AnimationEventData animationEventData = animationEvent.GetEventData();
                    if (Mathf.Approximately(animationEventData.TriggerTime, triggerTime))
                    {
                        if (animationEventData.SourceName == eventData.SourceName && animationEventData.EventIndex == eventIndex)
                        {
                            return;
                        }
                    }
                    else if (animationEventData.TriggerTime > triggerTime)
                    {
                        AnimationEvent newAnimationEvent = eventSource.CreateAnimationEvent(eventIndex, animationEventData);
                        if (newAnimationEvent != null)
                        {
                            m_AnimationEvents.Insert(i, newAnimationEvent);
                        }
                        else
                        {
                            Debug.LogError($"{eventSource.GetSourceName()}中没有找到事件{eventIndex}");
                        }
                        return;
                    }
                }
            }

            public void RemoveAnimationEvent(AnimationEventData eventData)
            {
                int index = FindAnimationEventIndex(eventData);
                if (index != -1)
                {
                    m_AnimationEvents.RemoveAt(index);
                }
            }

            public void RemoveAnimationEvent(IAnimationEventSource eventSource)
            {
                int i = 0;
                while (i < m_AnimationEvents.Count)
                {
                    if (m_AnimationEvents[i].GetEventSource() == eventSource)
                    {
                        int lastIndex = m_AnimationEvents.Count - 1;
                        (m_AnimationEvents[i], m_AnimationEvents[lastIndex]) = (m_AnimationEvents[lastIndex], m_AnimationEvents[i]);
                        Debug.LogWarning($"animation {m_AnimationAsset.name} removed event {m_AnimationEvents[i].GetEventData().EventIndex} because of {eventSource.GetSourceName()} unregistered");
                        m_AnimationEvents.RemoveAt(lastIndex);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            public void ReplaceAnimationEventArg(AnimationEventData eventData)
            {
                int index = FindAnimationEventIndex(eventData);
                if (index != -1)
                {
                    m_AnimationEvents[index].SetArg(eventData.EventArgs);
                }
            }

            private int FindAnimationEventIndex(AnimationEventData eventData)
            {
                for (int i = 0; i < m_AnimationEvents.Count; i++)
                {
                    AnimationEvent animationEvent = m_AnimationEvents[i];
                    if (animationEvent.GetEventData() == eventData)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
    }
}