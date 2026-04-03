using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using YBFramework.Common;

namespace YBFramework.Component
{
#if UNITY_EDITOR
    [DisplayName("动画管理器")]
#endif
    [Serializable]
    public sealed partial class AnimationManager : IComponent
    {
        [Serializable]
        private struct LayerData
        {
            public LayerType Layer;

            public string AvatarMaskName;
        }

        private static readonly List<Animation> s_AnimationTemps = new();

        [SerializeField] private LayerData[] m_LayerData;

        private string m_BoneTypeName;

        private PlayableGraph m_Graph;

        //TODO: graph的树状结构改变的节点：重新加载Owner Entity。装配某些带有动画的组件时。
        private AnimationLayerMixerPlayable m_LayerMixer;

        private AnimationMixer[] m_AnimationMixers;

        private readonly Dictionary<AnimationConnectionAsset, Animation[]> m_Connections = new();

        private readonly List<IAnimationEventSource> m_AnimationEventSources = new();

        public Entity GetOwner()
        {
            throw new NotImplementedException();
        }

        public void OnAddComponent(Entity entity)
        {
            throw new NotImplementedException();
        }

        public void OnRemoveComponent()
        {
            throw new NotImplementedException();
        }

        public void ResetComponent()
        {
            throw new NotImplementedException();
        }

        public IComponent Clone()
        {
            throw new NotImplementedException();
        }

        public void RegisterAnimationEventSource(IAnimationEventSource eventSource)
        {
            string sourceName = eventSource.GetSourceName();
            for (int i = 0; i < m_AnimationEventSources.Count; i++)
            {
                if (m_AnimationEventSources[i].GetSourceName() == sourceName)
                {
                    return;
                }
            }
            m_AnimationEventSources.Add(eventSource);
        }

        public void UnregisterAnimationEventSource(IAnimationEventSource eventSource)
        {
            if (m_AnimationEventSources.Remove(eventSource))
            {
                foreach (KeyValuePair<AnimationConnectionAsset, Animation[]> kvp in m_Connections)
                {
                    Animation[] animations = kvp.Value;
                    for (int i = 0; i < animations.Length; i++)
                    {
                        animations[i].RemoveAnimationEvent(eventSource);
                    }
                }
            }
        }

        public void RegisterConnection(AnimationConnectionAsset connectionAsset)
        {
            if (m_Connections.ContainsKey(connectionAsset))
            {
                return;
            }
            s_AnimationTemps.Clear();
            foreach (AnimationConnectionData connectionData in connectionAsset.GetAnimationConnectionData())
            {
                string animationAssetName = m_BoneTypeName + connectionData.ActionName;
                //TODO: 通过animationAssetName动画资源加载,现在使用new替代
                AnimationAsset animationAsset = ScriptableObject.CreateInstance<AnimationAsset>();
                Animation animation = Animation.Allocate(animationAsset, AnimationClipPlayable.Create(m_Graph, animationAsset.GetAnimationClip()));
                foreach (AnimationEventData eventData in animationAsset.GetAnimationEventData())
                {
                    IAnimationEventSource eventSource = FindAnimationEventSource(eventData.SourceName);
                    if (eventSource != null)
                    {
                        animation.AddAnimationEvent(eventData, eventSource);
                    }
                }
                AnimationMixer animationMixer = null;
                int animationMixerIndex = FindAnimationMixerIndex(connectionData.Layer);
                if (animationMixerIndex != -1)
                {
                    animationMixer = m_AnimationMixers[animationMixerIndex];
                }
                else
                {
                    for (int i = 0; i < m_LayerData.Length; i++)
                    {
                        if (m_LayerData[i].Layer == connectionData.Layer)
                        {
                            animationMixer = new AnimationMixer(m_Graph, connectionData.Layer);
                            int lastIndex = m_AnimationMixers?.Length ?? 0;
                            Array.Resize(ref m_AnimationMixers, lastIndex + 1);
                            m_LayerMixer.SetInputCount(lastIndex + 1);
                            m_LayerMixer.ConnectInput(lastIndex, animationMixer.GetMixer(), 0);
                            m_AnimationMixers[lastIndex] = animationMixer;
                            string avatarMaskName = m_LayerData[i].AvatarMaskName;
                            //TODO: 如果存在AvatarMaskName就加载AvatarMask，不存在不加载
                            if (!string.IsNullOrEmpty(avatarMaskName))
                            {
                                Debug.Log("加载AvatarMask:" + avatarMaskName);
                                AvatarMask avatarMask = new();
                                m_LayerMixer.SetLayerMaskFromAvatarMask((uint)lastIndex, avatarMask);
                            }
                            break;
                        }
                    }
                }
                if (animationMixer != null)
                {
                    AnimationPort animationPort = animationMixer.GetOrCreatePort(connectionData.PortName);
                    animation.ConnectedPort = animationPort;
                    animationPort.ConnectedAnimationCount++;
                    s_AnimationTemps.Add(animation);
                }
                else
                {
                    Debug.LogError($"{m_BoneTypeName}骨骼中不允许动画层{connectionData.Layer}");
                }
            }
            m_Connections.Add(connectionAsset, s_AnimationTemps.ToArray());
        }

        public void UnregisterConnection(AnimationConnectionAsset connectionAsset)
        {
            if (m_Connections.Remove(connectionAsset, out Animation[] animations))
            {
                for (int i = 0; i < animations.Length; i++)
                {
                    Animation animation = animations[i];
                    //TODO: 这里需要判断这个动画是否为当前连接的动画，如果是需要断开连接
                    animation.ConnectedPort.ConnectedAnimationCount--;
                    Animation.Free(animation);
                }
            }
        }

        public void RefreshGraph()
        {
            int i = 0;
            int oldCount = m_AnimationMixers.Length;
            int newCount = oldCount;
            while (i < newCount)
            {
                AnimationMixer animationMixer = m_AnimationMixers[i];
                animationMixer.RefreshPort();
                if (animationMixer.GetPortCount() <= 0)
                {
                    newCount--;
                    m_LayerMixer.DisconnectInput(newCount);
                    m_LayerMixer.ConnectInput(i, m_AnimationMixers[newCount].GetMixer(), 0);
                    (m_AnimationMixers[i], m_AnimationMixers[newCount]) = (m_AnimationMixers[newCount], m_AnimationMixers[i]);
                }
                else
                {
                    i++;
                }
            }
            if (oldCount != newCount)
            {
                m_LayerMixer.SetInputCount(newCount);
            }
        }

        public void ChangeConnection(AnimationConnectionAsset connectionAsset)
        {
            if (m_Connections.TryGetValue(connectionAsset, out Animation[] animations))
            {
                for (int i = 0; i < animations.Length; i++)
                {
                    Animation animation = animations[i];
                    //TODO: 这里考虑怎么连接到端口
                    animation.ConnectedPort.ChangeAnimation(animation);
                }
            }
        }

        public void ChangePort(LayerType layer, string portName)
        {
            int index = FindAnimationMixerIndex(layer);
            if (index != -1)
            {
                m_AnimationMixers[index].ChangePort(portName);
            }
        }

        public void SetSpeed(LayerType layer, string portName, float speed)
        {
            int index = FindAnimationMixerIndex(layer);
            if (index != -1)
            {
                m_AnimationMixers[index].SetSpeed(portName, speed);
            }
        }

        public void AddAnimationEvent(AnimationConnectionAsset connectionAsset, string actionName, AnimationEventData eventData)
        {
            if (m_Connections.TryGetValue(connectionAsset, out Animation[] animations))
            {
                string animationAssetName = m_BoneTypeName + actionName;
                for (int i = 0; i < animations.Length; i++)
                {
                    Animation animation = animations[i];
                    if (animation.GetAnimationAsset().name == animationAssetName)
                    {
                        IAnimationEventSource eventSource = FindAnimationEventSource(eventData.SourceName);
                        if (eventSource != null)
                        {
                            animation.AddAnimationEvent(eventData, eventSource);
                            return;
                        }
                    }
                }
            }
        }

        public void RemoveAnimationEvent(AnimationConnectionAsset connectionAsset, string actionName, AnimationEventData eventData)
        {
            if (m_Connections.TryGetValue(connectionAsset, out Animation[] animations))
            {
                string animationAssetName = m_BoneTypeName + actionName;
                for (int i = 0; i < animations.Length; i++)
                {
                    Animation animation = animations[i];
                    if (animation.GetAnimationAsset().name == animationAssetName)
                    {
                        animation.RemoveAnimationEvent(eventData);
                        return;
                    }
                }
            }
        }

        public void CoverAnimationEventParameters(AnimationConnectionAsset connectionAsset, string actionName, AnimationEventData eventData)
        {
            if (m_Connections.TryGetValue(connectionAsset, out Animation[] animations))
            {
                string animationAssetName = m_BoneTypeName + actionName;
                for (int i = 0; i < animations.Length; i++)
                {
                    Animation animation = animations[i];
                    if (animation.GetAnimationAsset().name == animationAssetName)
                    {
                        animation.CoverAnimationEventParameters(eventData);
                        return;
                    }
                }
            }
        }

        private IAnimationEventSource FindAnimationEventSource(string sourceName)
        {
            for (int i = 0; i < m_AnimationEventSources.Count; i++)
            {
                if (m_AnimationEventSources[i].GetSourceName() == sourceName)
                {
                    return m_AnimationEventSources[i];
                }
            }
            Debug.LogWarning("未找到对应事件源" + sourceName);
            return null;
        }

        private int FindAnimationMixerIndex(LayerType layer)
        {
            for (int i = 0; i < m_AnimationMixers.Length; i++)
            {
                if (m_AnimationMixers[i].GetLayerType() == layer)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}