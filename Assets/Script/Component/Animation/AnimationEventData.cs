using System;
using UnityEngine;

namespace YBFramework.Component
{
    [Serializable]
    public struct AnimationEventData : IEquatable<AnimationEventData>
    {
        [SerializeField] public string SourceName;

        [SerializeField] public int EventIndex;

        [SerializeField] public float TriggerTime;

        [SerializeReference] public AnimationEventArgs EventArgs;

        public static bool operator ==(AnimationEventData left, AnimationEventData right)
        {
            return left.SourceName == right.SourceName && left.EventIndex == right.EventIndex && Mathf.Approximately(left.TriggerTime, right.TriggerTime);
        }

        public static bool operator !=(AnimationEventData left, AnimationEventData right)
        {
            return !(left == right);
        }

        public bool Equals(AnimationEventData other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is AnimationEventData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SourceName, EventIndex, TriggerTime);
        }
    }
}