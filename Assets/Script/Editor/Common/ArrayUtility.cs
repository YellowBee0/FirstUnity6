using System.Collections.Generic;

namespace YBFramework.MyEditor.Common
{
    public static class ArrayUtility
    {
        public static T2 FindValueByOtherIndex<T1, T2>(IReadOnlyList<T1> from, IReadOnlyList<T2> to, T1 value)
        {
            if (from != null && to != null && from.Count == to.Count)
            {
                for (int i = 0; i < from.Count; i++)
                {
                    if (from[i] != null && from[i].Equals(value))
                    {
                        return to[i];
                    }
                }
            }
            return default;
        }
    }
}