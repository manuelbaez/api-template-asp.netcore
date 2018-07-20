using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace FieldsSelector
{
    public static class Selector
    {
        public static IEnumerable<string> GetPropertiesName(this Type member, IEnumerable<string> names)
        {
            List<string> propertiesName = new List<string>();
            if (names == null|| !names.Any()) return null;
            foreach (var name in names)
            {
                try
                {
                    propertiesName.Add(
                        member.GetProperty(name,
                            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).Name);
                }
                catch
                {
                    // ignored
                }
            }
            return propertiesName;
        }
        public static IEnumerable<object> Select<T>(IEnumerable<T> data, IEnumerable<string> fields)
        {
            var expression = new Func<dynamic, dynamic>(t =>
            {
                dynamic expandoObject = new ExpandoObject();
                if (fields != null)
                    foreach (var field in fields)
                    {
                        var value = t.GetType()
                            .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?
                            .GetValue(t, null);
                        (expandoObject as IDictionary<string, object>).Add(Char.ToLowerInvariant(field[0]) + field.Substring(1), value);
                    }
                return expandoObject;
            });
            return ((IEnumerable<object>)data).Select(expression);
        }
        public static object SelectObjectFields(this object data, IEnumerable<string> fields)
        {
            dynamic expandoObject = new ExpandoObject();
            if (fields != null)
                foreach (var field in fields)
                {
                    var value = data.GetType()
                        .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?
                        .GetValue(data, null);
                    (expandoObject as IDictionary<string, object>).Add(Char.ToLowerInvariant(field[0]) + field.Substring(1), value);
                }
            return expandoObject;
        }
        public static IQueryable<object> Select<T>(this IQueryable<T> data, IEnumerable<string> fields)
        {
            if (fields == null||!fields.Any())
                return (IQueryable<object>)data;

            var query = fields.Aggregate("t => new {", (current, field) => current + ($"t.{field},"));
            var lastQuery = query.Remove(query.Length - 1);
            lastQuery += "}";

            var cleanData = (IQueryable<object>)data.Select(lastQuery);

            return cleanData;
        }
    }
}
