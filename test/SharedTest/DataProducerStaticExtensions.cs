using Microsoft.Xrm.Sdk;
using System.Linq.Expressions;

namespace SharedTest;

internal static class DataProducerStaticExtensions
{
    internal static void EnsureValue<TE>(this TE entity, Expression<Func<TE, string>> selector, string defaultvalue)
        where TE : Entity
    {
        if (selector.Compile()(entity) == null)
        {
            SetActionFromSelector(selector)(entity, defaultvalue);
        }
    }

    internal static void EnsureValue<TE, TV>(this TE entity, Expression<Func<TE, TV>> selector, Expression<Func<TV>> defaultvalue)
        where TE : Entity
        where TV : class
    {
        if (selector.Compile()(entity) == null)
        {
            SetActionFromSelector(selector)(entity, defaultvalue.Compile()());
        }
    }

    internal static void EnsureValue<TE, TV>(this TE entity, Expression<Func<TE, EntityReference>> selector, TV defaultvalue)
        where TE : Entity
        where TV : Entity
    {
        if (selector.Compile()(entity) == null)
        {
            SetActionFromSelector(selector)(entity, defaultvalue.ToEntityReference());
        }
    }

    internal static void EnsureValue<TE, TV>(this TE entity, Expression<Func<TE, TV?>> selector, TV defaultvalue)
        where TE : Entity
        where TV : struct
    {
        if (selector.Compile()(entity) == null)
        {
            SetActionFromSelector(selector)(entity, defaultvalue);
        }
    }

    internal static void EnsureValue<TE, TV>(this TE entity, Expression<Func<TE, IEnumerable<TV>?>> selector, IEnumerable<TV> defaultvalue)
        where TE : Entity
        where TV : struct
    {
        if (selector.Compile()(entity) == null)
        {
            SetActionFromSelector(selector)(entity, defaultvalue);
        }
    }

    private static Action<TSetter, TValue> SetActionFromSelector<TSetter, TValue>(Expression<Func<TSetter, TValue>> selector)
    {
        var body = (MemberExpression)selector.Body ?? throw new NotSupportedException("Selector must have body");
        var entityParameterExpression = (ParameterExpression)(body.Expression ?? throw new NotSupportedException("Body on selector must have expression"));
        var valueParameterExpression = Expression.Parameter(typeof(TValue)) ?? throw new NotSupportedException("Must have parameter of type Value");
        return Expression.Lambda<Action<TSetter, TValue>>(Expression.Assign(selector.Body, valueParameterExpression), entityParameterExpression, valueParameterExpression).Compile();
    }
}