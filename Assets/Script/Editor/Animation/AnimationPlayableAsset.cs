using UnityEngine;
using UnityEngine.Playables;
using YBFramework.Component;

namespace YBFramework.MyEditor
{
    public class AnimationPlayableAsset : PlayableAsset
    {
        public AnimationAsset AnimationAsset;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return Playable.Create(graph);
        }
    }
}