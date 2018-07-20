using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Filtering.Constants
{
    public static class RegexDefinitions
    {
        public const string StringGroups = "^((?<v>\"{1}[^\"]*\"{1})+,?)+$";
        public const string Date = "^(/d{4}-/d{2}-/d{2})";
    }
}
