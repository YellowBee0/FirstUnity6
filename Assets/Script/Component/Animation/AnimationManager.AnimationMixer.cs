using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace YBFramework.Component
{
    public sealed partial class AnimationManager
    {
        private sealed class AnimationMixer
        {
            private readonly AnimationMixerPlayable m_Mixer;

            private readonly List<AnimationPort> m_Ports = new();

            private readonly List<(AnimationConnectionAsset animationSetAsset, Animation[] animations)> m_AnimationKits = new();

            private AnimationPort m_CurPort;

            private bool m_IsPlaying;

            public void RegisterAnimationKit(AnimationConnectionAsset animationConnectionAsset)
            {
                
            }

            private int FindPortIndex(string portName)
            {
                for (int i = 0; i < m_Ports.Count; i++)
                {
                    if (m_Ports[i].Name == portName)
                    {
                        return i;
                    }
                }
                return -1;
            }

            public void AddPort(string portName)
            {
                if (FindPortIndex(portName) == -1)
                {
                    AnimationPort port = new()
                    {
                        Name = portName
                    };
                    m_Ports.Add(port);
                }
            }

            public void RemovePort(string portName)
            {
                int index = FindPortIndex(portName);
                if (index != -1)
                {
                    m_Ports.RemoveAt(index);
                }
            }

            public void RefreshPorts()
            {
                m_Mixer.SetInputCount(m_Ports.Count);
                for (int i = 0; i < m_Ports.Count; i++)
                {
                    m_Mixer.DisconnectInput(i);
                    AnimationPort port = m_Ports[i];
                    Animation animation = port.GetCurAnimation();
                    m_Mixer.ConnectInput(i, animation?.GetCurAnimationClipPlayable() ?? default, 0, port.Weight);
                }
            }

            public void ChangePort(string portName)
            {
                if (m_CurPort != null)
                {
                    if (m_CurPort.Name == portName)
                    {
                        return;
                    }
                    int index = FindPortIndex(portName);
                    if (index != -1)
                    {
                        AnimationPort port = m_Ports[index];
                        Animation animation = port.GetCurAnimation();
                        if (animation != null)
                        {
                            m_Mixer.SetInputWeight(m_CurPort.GetIndex(), 0);
                            m_CurPort.Weight = 0;
                            Animation curAnimation = m_CurPort.GetCurAnimation();
                            if (curAnimation != null)
                            {
                                curAnimation.Pause();
                                curAnimation.Reset();
                            }
                            port.Weight = 1;
                            m_Mixer.SetInputWeight(port.GetIndex(), 1);
                            m_CurPort = port;
                            if (m_IsPlaying)
                            {
                                animation.Play();
                            }
                        }
                    }
                }
            }

            public void SetSpeed(string portName, float speed)
            {
                int index = FindPortIndex(portName);
                if (index != -1)
                {
                    m_Ports[index].SetSpeed(speed);
                }
            }

            public void Play()
            {
                if (m_IsPlaying)
                {
                    return;
                }
                m_CurPort.GetCurAnimation()?.Play();
                m_IsPlaying = true;
            }

            public void Pause()
            {
                if (m_IsPlaying)
                {
                    m_CurPort.GetCurAnimation()?.Pause();
                    m_IsPlaying = false;
                }
            }
        }
    }
}