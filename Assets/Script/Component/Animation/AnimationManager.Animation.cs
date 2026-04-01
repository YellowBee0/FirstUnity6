using UnityEngine.Animations;
using UnityEngine.Playables;

namespace YBFramework.Component
{
    public sealed partial class AnimationManager
    {
        private sealed class Animation
        {
            public AnimationAsset AnimationAsset;

            public AnimationClipPlayable AnimationClipPlayable;
                
            public AnimationPort ConnectedPort;

            //TODO:在这里加上动画事件的覆盖值

            private bool m_IsPlaying;

            public void Play()
            {
                if (m_IsPlaying)
                {
                    return;
                }
                AnimationClipPlayable.Play();
                m_IsPlaying = true;
            }

            public void Pause()
            {
                if (m_IsPlaying)
                {
                    AnimationClipPlayable.Pause();
                    m_IsPlaying = false;
                }
            }

            public void Reset()
            {
                AnimationClipPlayable.SetTime(0);
            }

            public void SetSpeed(float speed)
            {
                AnimationClipPlayable.SetSpeed(speed);
            }

            public bool IsPlaying()
            {
                return m_IsPlaying;
            }
        }
    }
}