using System;
using System.Collections.Generic;
using System.Text;

namespace Filtering.Enums
{
    public enum ConcatenationOperation
    {
        [Key("&&")]
        And,
        [Key("||")]
        Or
    }
}
