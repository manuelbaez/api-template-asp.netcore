using System;
using System.Collections.Generic;
using System.Text;

namespace Filtering.Enums
{
    [Flags]
    public enum GeneralOperation
    {
        [Key("==")]
        Equals = 0,
        [Key(">")]
        GreaterThan = 1,
        [Key("<")]
        LessThan = 1 << 1,
        [Key(">=")]
        GreaterOrEqualThan = 1 << 2,
        [Key("<=")]
        LessOrEqualThan = 1 << 3
    }
}
