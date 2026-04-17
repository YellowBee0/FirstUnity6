using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace YBFramework.Component
{
    public sealed partial class AnimationManager
    {
        private sealed class AnimationMixer
        {
            private sealed class AnimationPortNode
            {
                private static readonly Queue<AnimationPortNode> s_Pool = new();

                public static AnimationPortNode Allocate()
                {
                    return s_Pool.Count > 0 ? s_Pool.Dequeue() : new AnimationPortNode();
                }

                public static void Free(AnimationPortNode node)
                {
                    node.Port = null;
                    s_Pool.Enqueue(node);
                }

                public AnimationPort Port;

                public AnimationPortNode NextNode;
            }

            private readonly List<AnimationPort> m_Ports = new();

            private readonly AnimationMixerPlayable m_Mixer;

            private readonly LayerType m_LayerType;

            private AnimationPort m_CurPort;

            private AnimationPortNode m_CrossPortNode;

            private float m_TotalDeltaWeight;

            private int m_CrossPortCount;

            private bool m_IsPlaying;

            private bool m_IsCrossing;

            public AnimationMixer(PlayableGraph graph, LayerType layerType)
            {
                m_Mixer = AnimationMixerPlayable.Create(graph);
                m_LayerType = layerType;
            }

            public int GetPortCount()
            {
                return m_Ports.Count;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public AnimationMixerPlayable GetMixer()
            {
                return m_Mixer;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public LayerType GetLayerType()
            {
                return m_LayerType;
            }

            public AnimationPort GetOrCreatePort(string portName)
            {
                int index = FindPortIndex(portName);
                if (index == -1)
                {
                    index = m_Ports.Count;
                    AnimationPort port = new(this, index)
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
                        m_Ports[i].Disconnect();
                        m_Ports[lastIndex].SetIndex(i);
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
                if (m_IsCrossing)
                {
                    Animation animation = m_CurPort.GetCurConnectedAnimation();
                    if (animation.GetAnimationAsset().GetAnimationClip().isLooping)
                    {
                        animation.Play();
                    }
                    AnimationPortNode curNode = m_CrossPortNode;
                    while (curNode != null)
                    {
                        animation = curNode.Port.GetCurConnectedAnimation();
                        if (animation.GetAnimationAsset().GetAnimationClip().isLooping)
                        {
                            animation.Play();
                        }
                        curNode = curNode.NextNode;
                    }
                    UpdateCross().Forget();
                }
                else
                {
                    m_CurPort.GetCurConnectedAnimation()?.Play();
                }
                m_IsPlaying = true;
            }

            public void Pause()
            {
                if (m_IsPlaying)
                {
                    if (m_IsCrossing)
                    {
                        m_CurPort.GetCurConnectedAnimation().Pause();
                        AnimationPortNode curNode = m_CrossPortNode;
                        while (curNode != null)
                        {
                            curNode.Port.GetCurConnectedAnimation().Pause();
                            curNode = curNode.NextNode;
                        }
                    }
                    m_CurPort.GetCurConnectedAnimation()?.Pause();
                    m_IsPlaying = false;
                }
            }

            public void ChangePort(string portName)
            {
                if (m_CurPort != null && m_CurPort.Name == portName)
                {
                    return;
                }
                InterruptCross();
                int index = FindPortIndex(portName);
                if (index != -1)
                {
                    AnimationPort port = m_Ports[index];
                    Animation animation = port.GetCurConnectedAnimation();
                    if (animation != null)
                    {
                        if (m_CurPort != null)
                        {
                            m_CurPort.SetWeight(0);
                            Animation curAnimation = m_CurPort.GetCurConnectedAnimation();
                            if (curAnimation != null)
                            {
                                curAnimation.Pause();
                                curAnimation.Reset();
                            }
                        }
                        port.SetWeight(1);
                        m_CurPort = port;
                        if (m_IsPlaying)
                        {
                            animation.Play();
                        }
                    }
                }
            }

            public void ChangePortWithCross(string portName)
            {
                if (m_CurPort != null && m_CurPort.Name == portName)
                {
                    return;
                }
                AnimationPort targetPort = null;
                if (m_IsCrossing)
                {
                    AnimationPortNode curNode = m_CrossPortNode;
                    AnimationPortNode preNode = null;
                    while (curNode != null)
                    {
                        if (curNode.Port.Name == portName)
                        {
                            if (preNode != null)
                            {
                                preNode.NextNode = curNode.NextNode;
                            }
                            else
                            {
                                m_CrossPortNode = curNode.NextNode;
                            }
                            targetPort = curNode.Port;
                            AnimationPortNode.Free(curNode);
                            m_CrossPortCount--;
                            break;
                        }
                        preNode = curNode;
                        curNode = curNode.NextNode;
                    }
                }
                if (targetPort == null)
                {
                    int index = FindPortIndex(portName);
                    if (index != -1)
                    {
                        AnimationPort port = m_Ports[index];
                        if (port.GetCurConnectedAnimation() != null)
                        {
                            targetPort = port;
                        }
                    }
                }
                if (targetPort != null)
                {
                    AnimationPortNode newNode = AnimationPortNode.Allocate();
                    newNode.Port = m_CurPort;
                    newNode.NextNode = m_CrossPortNode;
                    m_CrossPortNode = newNode;
                    m_CrossPortCount++;
                    m_TotalDeltaWeight = 1 - targetPort.GetWeight();
                    m_CurPort = targetPort;
                    if (m_IsPlaying && !m_IsCrossing)
                    {
                        m_IsCrossing = true;
                        UpdateCross().Forget();
                    }
                }
            }

            public void InterruptCross()
            {
                if (m_IsCrossing)
                {
                    AnimationPortNode curNode = m_CrossPortNode;
                    while (curNode != null)
                    {
                        curNode.Port.SetWeight(0);
                        Animation animation = curNode.Port.GetCurConnectedAnimation();
                        animation.Pause();
                        animation.Reset();
                        AnimationPortNode.Free(curNode);
                        curNode = curNode.NextNode;
                    }
                    m_CrossPortNode = null;
                    m_CrossPortCount = 0;
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

            //TODO: 测试全局timescale改变后，这个过渡时间是不是也会跟着改变
            private async UniTaskVoid UpdateCross()
            {
                await UniTask.NextFrame();
                while (m_IsPlaying && m_IsCrossing)
                {
                    float crossPortTotalSpeed = 0;
                    AnimationPortNode curNode = m_CrossPortNode;
                    while (curNode != null)
                    {
                        crossPortTotalSpeed += curNode.Port.GetSpeed();
                        curNode = curNode.NextNode;
                    }
                    float averageSpeed = (crossPortTotalSpeed + m_CurPort.GetSpeed()) / (m_CrossPortCount + 1);
                    float increaseWight = Time.deltaTime * averageSpeed / m_TotalDeltaWeight;
                    curNode = m_CrossPortNode;
                    AnimationPortNode preNode = null;
                    while (curNode != null)
                    {
                        AnimationPort port = curNode.Port;
                        port.SetWeight(port.GetWeight() - increaseWight * crossPortTotalSpeed / port.GetSpeed());
                        if (port.GetWeight() <= 0)
                        {
                            if (preNode != null)
                            {
                                preNode.NextNode = curNode.NextNode;
                            }
                            else
                            {
                                m_CrossPortNode = curNode.NextNode;
                            }
                            Animation animation = port.GetCurConnectedAnimation();
                            animation.Pause();
                            animation.Reset();
                            m_CrossPortCount--;
                            AnimationPortNode.Free(curNode);
                        }
                        else
                        {
                            preNode = curNode;
                        }
                        curNode = curNode.NextNode;
                    }
                    if (m_CrossPortCount <= 0)
                    {
                        m_IsCrossing = false;
                        m_CurPort.SetWeight(1);
                    }
                    else
                    {
                        m_CurPort.SetWeight(m_CurPort.GetWeight() + increaseWight);
                    }
                    await UniTask.NextFrame();
                }
            }
        }
    }
}