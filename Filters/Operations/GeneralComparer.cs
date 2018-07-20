using Filtering.Enums;
using Filtering.General;
using System.Collections.Generic;
using System.Linq;

namespace Filtering.Operations
{
    static class GeneralComparer
    {
        public static (string query, object[] values) GetQuery(IEnumerable<FilterProperty<GeneralOperation>> properties,string querySeed, IEnumerable<object> valuesSeed)
        {
            var queryData = new List<dynamic>(valuesSeed);
            int paramNumber = valuesSeed.Count();

            var queryString = properties.Aggregate(querySeed, (current, item) =>
            {
                var _newString = current + item.ConcatenationOperation.GetKey() + item.Key + item.Operation.GetKey() + $"@{paramNumber}";
                queryData.Add(item.Value);
                paramNumber++;
                return _newString;
            });    
            return (queryString, queryData.ToArray());
        }
    }
}
