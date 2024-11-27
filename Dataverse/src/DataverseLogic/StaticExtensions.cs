using Microsoft.Xrm.Sdk;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace DataverseLogic;

public static class StaticExtensions
{
    public static T? GetOrDefault<T>(this ParameterCollection keyValueCollection, string key)
    {
        if (keyValueCollection == null)
        {
            return default;
        }

        if (keyValueCollection.TryGetValue(key, out var value))
        {
            return (T)value;
        }

        return default;
    }

#pragma warning disable CS8669 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context. Auto-generated code requires an explicit '#nullable' directive in source.
    public static T ShallowCopy<T>(this T target, params Expression<Func<T, object?>>[] attributesExceptions)
        where T : Entity, new()
    {
        var copyEntity = new T();

        if (target != null)
        {
            var attributeExceptions = GetAttributeExceptionStrings(attributesExceptions);

            foreach (var attr in target.Attributes)
            {
                if (attributeExceptions.Contains(attr.Key, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                copyEntity[attr.Key] = attr.Value;
            }
        }

        return copyEntity;
    }

    private static Collection<string?> GetAttributeExceptionStrings<T>(params Expression<Func<T, object?>>[] lambdas)
        where T : Entity, new()
    {
        var attributeExceptions = new Collection<string?>();
        if (lambdas != null)
        {
            foreach (var lambda in lambdas)
            {
                attributeExceptions.Add(GetMemberName(lambda));
            }
        }

        return attributeExceptions;
    }

    private static string? GetMemberName<T>(Expression<Func<T, object?>> lambda)
         where T : Entity, new()
    {
        MemberExpression? body = lambda.Body as MemberExpression;

        if (body == null)
        {
            UnaryExpression ubody = (UnaryExpression)lambda.Body;
            body = ubody.Operand as MemberExpression;
        }

        return body?.Member.Name;
    }
#pragma warning restore CS8669 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context. Auto-generated code requires an explicit '#nullable' directive in source.
}