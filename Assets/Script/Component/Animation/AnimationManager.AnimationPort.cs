using System.Collections.Generic;

namespace YBFramework.Component
{
    public sealed partial class AnimationManager
    {
        private sealed class AnimationPort
        {
            private readonly List<Animation> m_ConnectedAnimations = new();

            private Animation m_CurAnimation;

            private float m_Speed;

            private int m_Index;

            public int ConnectedAnimationCount;

            public float Weight;

            public string Name;

            public void ConnectAnimation(Animation animation)
            {
                animation.ConnectedPort = this;
                m_ConnectedAnimations.Add(animation);
            }

            public void DisconnectAnimation(Animation animation)
            {
                if (m_ConnectedAnimations.Remove(animation))
                {
                    if (m_CurAnimation == animation)
                    {
                        Animation newAnimation = null;
                        if (m_ConnectedAnimations.Count > 0)
                        {
                            if (m_CurAnimation != null)
                            {
                                m_CurAnimation.Pause();
                                m_CurAnimation.Reset();
                            }
                            newAnimation = m_ConnectedAnimations[^1];
                        }
                        m_CurAnimation = newAnimation;
                    }
                }
            }

            public void ChangeAnimation(Animation animation)
            {
                if (m_CurAnimation == animation)
                {
                    return;
                }
                if (m_CurAnimation != null)
                {
                    m_CurAnimation.Pause();
                    m_CurAnimation.Reset();
                }
                animation?.SetSpeed(m_Speed);
                m_CurAnimation = animation;
            }

            public void SetSpeed(float speed)
            {
                m_CurAnimation?.SetSpeed(speed);
                m_Speed = speed;
            }

            public int GetIndex()
            {
                return m_Index;
            }
        }
    }
}