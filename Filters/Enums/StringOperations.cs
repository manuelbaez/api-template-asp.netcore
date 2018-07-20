using System;

namespace Filtering.Enums
{
    [Flags]
    public enum StringOperation
    {
        [Key(".Equals(")]
        Equals = 0,
        [Key(".StartsWith(")]
        StartsWith = 1 << 0,
        [Key(".EndsWith(")]
        EndsWith = 1 << 1,
        SpaceLike = 1 << 2,
        [Key(".Contains(")]
        Like = 1 << 3
    }
}
