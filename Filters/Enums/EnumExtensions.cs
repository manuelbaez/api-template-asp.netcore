using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Filtering.Enums
{
    public static class EnumExtensions
    {
        public static List<EnumValue> GetValues<T>()
        {
            List<EnumValue> values = new List<EnumValue>();
            foreach (var itemType in Enum.GetValues(typeof(T)))
            {
                //For each value of this enumeration, add a new EnumValue instance
                values.Add(new EnumValue()
                {
                    Name = Enum.GetName(typeof(T), itemType),
                    Description = itemType.GetKey(),
                    Value = (int)itemType
                });
            }
            return values;
        }
    }

    public static class EnumHelper
    {
        public static string GetKey(this object o)
        {
            var fields = o.GetType().GetFields();

            var field = fields.FirstOrDefault(t => t.GetValue(o).Equals(o));
            if (field == null) return null;

            var attribute = field.GetCustomAttribute(typeof(KeyAttribute)) as KeyAttribute;
            return attribute != null ? attribute.Name : field.Name;
        }
        public static string GetMemberByKey<TEnum>(string key)
        {
            var fields = typeof(TEnum).GetFields();

            var attributes = fields.Select(t => new
            {
                Name = t.Name,
                Attribute = (KeyAttribute)t.GetCustomAttribute(typeof(KeyAttribute))
            });

            return attributes.FirstOrDefault(t => t.Attribute.Name == key)?.Name;
        }
    }

    public static class EnumerableExpressionHelper
    {
        public static Expression<Func<TSource, String>> CreateEnumToStringExpression<TSource, TMember>(
            Expression<Func<TSource, TMember>> memberAccess, string defaultValue = "")
        {
            var type = typeof(TMember);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException("TMember must be an Enum type");
            }

            var enumNames = Enum.GetNames(type);
            var enumValues = (TMember[])Enum.GetValues(type);

            var inner = (Expression)Expression.Constant(defaultValue);

            var parameter = memberAccess.Parameters[0];

            for (int i = 0; i < enumValues.Length; i++)
            {
                inner = Expression.Condition(
                Expression.Equal(memberAccess.Body, Expression.Constant(enumValues[i])),
                Expression.Constant(enumNames[i]),
                inner);
            }

            var expression = Expression.Lambda<Func<TSource, String>>(inner, parameter);

            return expression;
        }
    }

    public class KeyAttribute : Attribute
    {
        public string Name { get; }
        public KeyAttribute(string name)
        {
            this.Name = name;
        }
    }

    public class EnumValue
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }
}