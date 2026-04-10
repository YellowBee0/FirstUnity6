using UnityEngine;
using UnityEngine.Timeline;

namespace YBFramework.MyEditor
{
    [TrackColor(0.9f, 0.9f, 0.9f)]
    [TrackClipType(typeof(AnimationPlayableAsset))]
    [TrackBindingType(typeof(GameObject))]
    public class AnimationPreviewTrackAsset : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            AnimationPlayableAsset asset = clip.asset as AnimationPlayableAsset;
            if (asset)
            {
                
            }
        }
    }
}