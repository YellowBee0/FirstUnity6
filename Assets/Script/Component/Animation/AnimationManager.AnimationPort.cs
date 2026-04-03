namespace YBFramework.Component
{
    public sealed partial class AnimationManager
    {
        private sealed class AnimationPort
        {
            private readonly AnimationMixer m_Mixer;

            private Animation m_CurAnimation;

            private float m_Speed;

            public int Index;

            public int ConnectedAnimationCount;

            public float Weight;

            public string Name;

            public AnimationPort(AnimationMixer mixer)
            {
                m_Mixer = mixer;
            }

            public Animation GetCurAnimation()
            {
                return m_CurAnimation;
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
                m_Mixer.ChangePortConnectedAnimation(this, animation);
                animation.SetSpeed(m_Speed);
                m_CurAnimation = animation;
            }

            public void SetSpeed(float speed)
            {
                m_CurAnimation.SetSpeed(speed);
                m_Speed = speed;
            }
        }
    }
}