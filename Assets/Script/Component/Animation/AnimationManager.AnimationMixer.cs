using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace YBFramework.Component
{
    public sealed partial class AnimationManager
    {
        private sealed class AnimationMixer
        {
            private readonly List<AnimationPort> m_Ports = new();

            private readonly AnimationMixerPlayable m_Mixer;

            private readonly LayerType m_LayerType;

            private AnimationPort m_CurPort;

            private bool m_IsPlaying;

            public AnimationMixer(PlayableGraph graph, LayerType layerType)
            {
                m_Mixer = AnimationMixerPlayable.Create(graph);
                m_LayerType = layerType;
            }

            public int GetPortCount()
            {
                return m_Ports.Count;
            }

            public AnimationMixerPlayable GetMixer()
            {
                return m_Mixer;
            }

            public LayerType GetLayerType()
            {
                return m_LayerType;
            }

            public AnimationPort GetOrCreatePort(string portName)
            {
                int index = FindPortIndex(portName);
                if (index == -1)
                {
                    AnimationPort port = new(this)
                    {
                        Name = portName
                    };
                    m_Ports.Add(port);
                }
                return m_Ports[index];
            }

            public void RefreshPort()
            {
                int i = 0;
                int oldCount = m_Ports.Count;
                while (i < m_Ports.Count)
                {
                    if (m_Ports[i].ConnectedAnimationCount <= 0)
                    {
                        int lastIndex = m_Ports.Count - 1;
                        AnimationPort lastPort = m_Ports[lastIndex];
                        m_Mixer.DisconnectInput(lastPort.Index);
                        Animation lastAnimation = lastPort.GetCurAnimation();
                        if (lastAnimation != null)
                        {
                            m_Mixer.ConnectInput(i, lastAnimation.GetAnimationClipPlayable(), 0, lastPort.Weight);
                        }
                        lastPort.Index = i;
                        (m_Ports[i], m_Ports[lastIndex]) = (m_Ports[lastIndex], m_Ports[i]);
                        m_Ports.RemoveAt(lastIndex);
                    }
                    else
                    {
                        i++;
                    }
                }
                int newCount = m_Ports.Count;
                if (newCount != oldCount)
                {
                    m_Mixer.SetInputCount(newCount);
                }
            }

            public void ChangePort(string portName)
            {
                if (m_CurPort != null && m_CurPort.Name == portName)
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
                        if (m_CurPort != null)
                        {
                            m_Mixer.SetInputWeight(m_CurPort.Index, 0);
                            m_CurPort.Weight = 0;
                            Animation curAnimation = m_CurPort.GetCurAnimation();
                            if (curAnimation != null)
                            {
                                curAnimation.Pause();
                                curAnimation.Reset();
                            }
                        }
                        port.Weight = 1;
                        m_Mixer.SetInputWeight(port.Index, 1);
                        m_CurPort = port;
                        if (m_IsPlaying)
                        {
                            animation.Play();
                        }
                    }
                }
            }

            public void ChangePortConnectedAnimation(AnimationPort port, Animation animation)
            {
                m_Mixer.DisconnectInput(port.Index);
                m_Mixer.ConnectInput(port.Index, animation.GetAnimationClipPlayable(), 0, port.Weight);
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
        }
    }
}