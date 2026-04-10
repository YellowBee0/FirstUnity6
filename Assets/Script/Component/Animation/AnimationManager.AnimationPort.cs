using System.Runtime.CompilerServices;
using UnityEngine.Playables;

namespace YBFramework.Component
{
    public sealed partial class AnimationManager
    {
        private sealed class AnimationPort
        {
            private readonly AnimationMixer m_Mixer;

            private Animation m_CurConnectedAnimation;

            private float m_Speed;

            private float m_Weight;

            private int m_Index;

            public int ConnectedAnimationCount;

            public string Name;

            public AnimationPort(AnimationMixer mixer, int index)
            {
                m_Mixer = mixer;
                m_Index = index;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetWeight()
            {
                return m_Weight;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetSpeed()
            {
                return m_Speed;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Animation GetCurConnectedAnimation()
            {
                return m_CurConnectedAnimation;
            }

            public void SetWeight(float weight)
            {
                if (weight > 1)
                {
                    weight = 1;
                }
                else if (weight < 0)
                {
                    weight = 0;
                }
                m_Weight = weight;
                m_Mixer.GetMixer().SetInputWeight(m_Index, weight);
            }

            public void SetSpeed(float speed)
            {
                m_CurConnectedAnimation.SetSpeed(speed);
                m_Speed = speed;
            }

            public void SetIndex(int index)
            {
                if (m_Index != index)
                {
                    Disconnect();
                    m_Index = index;
                    Connect(m_CurConnectedAnimation);
                }
            }
            
            public void Connect(Animation animation)
            {
                m_Mixer.InterruptCross();
                animation.SetSpeed(m_Speed);
                m_Mixer.GetMixer().ConnectInput(m_Index, animation.GetAnimationClipPlayable(), 0, m_Weight);
                m_CurConnectedAnimation = animation;
            }

            public void Disconnect()
            {
                if (m_CurConnectedAnimation != null)
                {
                    m_Mixer.InterruptCross();
                    m_CurConnectedAnimation.Pause();
                    m_CurConnectedAnimation.Reset();
                    m_CurConnectedAnimation = null;
                    m_Mixer.GetMixer().DisconnectInput(m_Index);
                }
            }
        }
    }
}