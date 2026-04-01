using System;
using System.Collections.Generic;
using System.Reflection;
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
        private PlayableGraph m_Graph;

        private AnimationLayerMixerPlayable m_LayerMixer;

        private Dictionary<IAnimationEventSource, (string eventName, MethodInfo methodInfo)[]> m_AnimationEventSources;

        private string m_BoneTypeName;
        
        private readonly List<(AnimationConnectionAsset connectionAsset, Animation[] animations)> m_Connections = new();

        public void RegisterAnimationEventSource(IAnimationEventSource eventSource)
        {
            m_AnimationEventSources ??= new Dictionary<IAnimationEventSource, (string eventName, MethodInfo methodInfo)[]>();
            if (!m_AnimationEventSources.ContainsKey(eventSource))
            {
                m_AnimationEventSources.Add(eventSource, eventSource.GetAnimationEventMethodInfos());
            }
        }

        public void UnregisterAnimationEventSource(IAnimationEventSource eventSource)
        {
            m_AnimationEventSources?.Remove(eventSource);
        }

        public void RegisterConnection(AnimationConnectionAsset animationConnectionAsset)
        {
            foreach (AnimationConnectionData connectionData in animationConnectionAsset.GetAnimationConnectionData())
            {
                
            }
        }

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
    }
}