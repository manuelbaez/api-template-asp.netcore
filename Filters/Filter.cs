using Filtering.Constants;
using Filtering.Enums;
using Filtering.General;
using Filtering.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Filtering
{
    public static class Filter
    {
        private static IEnumerable<QueryProperty<string>> FilterQuery(IDictionary<string, string> query)
        {
            var filteredQuery = new List<QueryProperty<string>>();

            foreach (var item in query)
            {

                if (item.Value.ToString().StartsWith("="))
                {
                    filteredQuery.Add(new QueryProperty<string>
                    {
                        Key = item.Key.ToString() + "=",
                        Value = item.Value.ToString().Replace("=", ""),
                        ConcatenationOperation = ConcatenationOperation.And
                    });
                    continue;
                }


                var pattern = new Regex(RegexDefinitions.StringGroups);
                var expressionMatches = pattern.Match(item.Value);
                var groups = expressionMatches.Groups["v"].Captures.Cast<Capture>().Select(t => t.Value);

                if (groups.Any())
                {
                    foreach (var group in groups) filteredQuery.Add(new QueryProperty<string>
                    {
                        Key = item.Key,
                        Value = group,
                        ConcatenationOperation = ConcatenationOperation.Or
                    });
                    continue;
                }


                if (item.Value.Contains(","))
                {
                    var values = item.Value.Split(",");
                    foreach (var value in values) filteredQuery.Add(new QueryProperty<string>
                    {
                        Key = item.Key,
                        Value = value,
                        ConcatenationOperation = ConcatenationOperation.Or
                    });
                    continue;
                }


                filteredQuery.Add(new QueryProperty<string>
                {
                    Key = item.Key,
                    Value = item.Value,
                    ConcatenationOperation = ConcatenationOperation.And
                });
            }
            return filteredQuery;

        }
        private static IEnumerable<QueryProperty<dynamic>> DeserializePropertiesValue(IEnumerable<QueryProperty<string>> query)
        {
            var filters = query.Select(t => new QueryProperty<dynamic>
            {
                Key = t.Key,
                Value = (dynamic)JsonConvert.DeserializeObject(t.Value),
                ConcatenationOperation = t.ConcatenationOperation
            });
            return filters;
        }
        private static void AddFilter(this List<FilterProperty<StringOperation>> stringFilters, string propertyName, QueryProperty<dynamic> filter)
        {

            var operation = (StringOperation)(
                ((filter.Value.EndsWith("$$") ? 1 : 0) << 0) |
                ((filter.Value.StartsWith("$$") ? 1 : 0) << 1) |
                ((filter.Value.StartsWith("*") ? 1 : 0) << 2) |
                (((filter.Value.StartsWith("$") && filter.Value.EndsWith("$")) ? 1 : 0) << 3)
                );
            filter.Value = filter.Value.Replace("*", "").Replace("$", "");
            if (operation == StringOperation.SpaceLike)
            {
                var values = filter.Value.Replace("*", "").Split(" ");
                foreach (var value in values)
                {
                    stringFilters.Add(new FilterProperty<StringOperation>
                    {
                        Key = propertyName,
                        Value = value,
                        Operation = StringOperation.Like,
                        ConcatenationOperation = ConcatenationOperation.And
                    });
                }
                return;
            }
            stringFilters.Add(new FilterProperty<StringOperation>
            {
                Key = propertyName,
                Value = filter.Value,
                Operation = operation,
                ConcatenationOperation = filter.ConcatenationOperation
            });
        }

        public static Expression<Func<T, bool>> GetExpression<T>(this IDictionary<string, string> queryValues) where T : class
        {
            if (!queryValues.Any()) return null;
            var filteredQuery = FilterQuery(queryValues);
            var filters = DeserializePropertiesValue(filteredQuery);

            var memberType = typeof(T);

            var stringFilters = new List<FilterProperty<StringOperation>>();
            var generalFilters = new List<FilterProperty<GeneralOperation>>();


            foreach (var filter in filters)
            {
                var propertyName = memberType.GetProperty(filter.Key
                    .Replace(">", "")
                    .Replace("<", "")
                    .Replace("=", ""),
                   BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?.Name;

                if (string.IsNullOrEmpty(propertyName))
                    continue;

                if (filter.Value is string)
                {
                    stringFilters.AddFilter(propertyName, filter);
                }
                else
                {
                    var operation = (GeneralOperation)(
                                ((filter.Key.EndsWith(GeneralOperation.GreaterThan.GetKey()) ? 1 : 0)) |
                                ((filter.Key.EndsWith(GeneralOperation.LessThan.GetKey()) ? 1 : 0) << 1) |
                                ((filter.Key.EndsWith(GeneralOperation.GreaterOrEqualThan.GetKey()) ? 1 : 0) << 2) |
                                ((filter.Key.EndsWith(GeneralOperation.LessOrEqualThan.GetKey()) ? 1 : 0) << 3)
                               );
                    generalFilters.Add(new FilterProperty<GeneralOperation>
                    {
                        Key = propertyName,
                        Value = filter.Value,
                        Operation = operation,
                        ConcatenationOperation = filter.ConcatenationOperation
                    });
                }
            }
            if (!stringFilters.Any() && !generalFilters.Any()) return null;
            var filterQuery = Operations.StringComparer.GetQuery(stringFilters, "", new List<object>());
            filterQuery = GeneralComparer.GetQuery(generalFilters, filterQuery.query, filterQuery.values);
            filterQuery.query = filterQuery.query.Substring(2, filterQuery.query.Length - 2);

            var expression = DynamicExpressionParser.ParseLambda<T, bool>(null, false, filterQuery.query, filterQuery.values);
            return expression;
        }

        public static IQueryable<T> FilterData<T>(this IQueryable<T> data, IDictionary<string, string> queryValues) where T : class
        {
            if (!queryValues.Any()) return data;
            var filterExpression = queryValues.GetExpression<T>();
            if (filterExpression is null) return data;
            return data.Where(filterExpression);
        }
    }
}
