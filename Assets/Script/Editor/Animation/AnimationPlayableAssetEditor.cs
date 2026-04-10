using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    [CustomTimelineEditor(typeof(AnimationPlayableAsset))]
    public class AnimationPlayableAssetEditor : ClipEditor
    {
        private AnimationPlayableAsset m_PlayableAsset;

        public override void OnClipChanged(TimelineClip clip)
        {
            m_PlayableAsset = clip.asset as AnimationPlayableAsset;
            if (!m_PlayableAsset)
            {
                Debug.LogError("Clip asset is not an AnimationPlayableAsset.");
            }
        }

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            foreach (AnimationEventData eventData in m_PlayableAsset.AnimationAsset.GetAnimationEventData())
            {
                float normalized = (float)(eventData.TriggerTime / clip.duration);
                float x = region.position.x + normalized * region.position.width;
                GUI.Box(new Rect(x, region.position.y, 5, 20), "");
            }
        }
    }
}