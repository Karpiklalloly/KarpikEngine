using System;

namespace Karpik.StatAndAbilities
{
    [Flags]
    public enum BuffRange
    {
        Min = 1,
        Max = 2,
        Value = 4,
        All = Min | Max | Value
    }

    public static class BuffRangeExtensions
    {
        public static bool Flagged(this BuffRange range, BuffRange flag) => (range & flag) == flag;
    }
}