using Microsoft.Xrm.Sdk;
using System.Linq.Expressions;

namespace SharedDataverseLogic.Utility;

/// <summary>
/// This class is usefull in cases where you receive information that shall update an existing record,
/// but you want to only update the attributes that have actually changed to minimize plugin executions and other side effects of updates in Dataverse.
/// It handles create by supplying a null as entityOriginal.
/// Mapping is easy to read as all 'plumbing' is handled by the class.
/// Sample usage:
/// public (Contact DeltaUpdate, bool HasChanges) MapCprDataToContactAsDemo(Contact original, PersonDataAsDemo dto)
///     {
///          return new XrmDeltaMapper &lt; Contact, PersonDataAsDemo &gt;(original, dto)
///             .MapAttribute(e => e.FirstName, d => d.Fornavne, (v) => v?.ToUpper(culture: CultureInfo.CurrentCulture))
///             .MapAttribute(e => e.LastName, d => d.Efternavn)
///             .MapAttribute(e => e.ParentCustomerId, d => d.Afdeling, ResolveParentByDepartment)
///             .MapAttribute(e => e.NumberOfChildren, d => d.AntalBoern)
///             .MapAttribute(e => e.BirthDate, d => d.BirthDate)
///             .MapAttribute(e => e.GenderCode, d => d.Koen, (v) => v switch {
///                 KoensMuligheder.Mand => Contact_GenderCode.Male,
///                 KoensMuligheder.Kvinde => Contact_GenderCode.Female,
///                 _ => (Contact_GenderCode?)null,
///             })
///             .Result();
///     }
/// </summary>
/// <typeparam name="TE">Type of the entity/table</typeparam>
/// <typeparam name="TI">Type of the dto</typeparam>
/// <param name="entityOriginal">Current state of the record</param>
/// <param name="inputDto">The dto with input data that may cause updates</param>
public class XrmDeltaMapper<TE, TI>(TE? entityOriginal, TI inputDto)
    where TE : Entity, new()
    where TI : class
{
    private readonly TE entityDelta = new TE();
    private bool hasChanges;

    public (TE EntityDelta, bool HasChanges) Result()
    {
        if (entityOriginal != null && entityOriginal.Id != Guid.Empty)
        {
            entityDelta.Id = entityOriginal.Id;
        }

        return (entityDelta, hasChanges);
    }

    public XrmDeltaMapper<TE, TI> MapAttribute(Expression<Func<TE, string?>> entityAttributePicker, Func<TI, string?> dtoAttributePicker, Func<string?, string?>? resolver = null)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(entityAttributePicker, nameof(entityAttributePicker));
        ArgumentNullExceptionHelper.ThrowIfNull(dtoAttributePicker, nameof(dtoAttributePicker));

        if (inputDto == null)
            return this;
        var valueResolver = resolver ?? (s => s);
        string? resolvedInputValue = valueResolver(dtoAttributePicker(inputDto));
        string? originalValue = null;
        bool doUpdate = false;
        if (entityOriginal == null)
        {
            doUpdate = true;
        }
        else
        {
            originalValue = entityAttributePicker.Compile()(entityOriginal);
            doUpdate = resolvedInputValue == null
                ? originalValue != null
                : !resolvedInputValue.Equals(originalValue, StringComparison.OrdinalIgnoreCase);
        }

        if (doUpdate)
        {
            var setter = SetValueAction(entityAttributePicker);
            setter(entityDelta, resolvedInputValue);
            hasChanges = true;
        }

        return this;
    }

    public XrmDeltaMapper<TE, TI> MapAttribute<T>(Expression<Func<TE, T?>> entityAttributePicker, Func<TI, T?> dtoAttributePicker, Func<T?, T?>? resolver = null)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(entityAttributePicker, nameof(entityAttributePicker));
        ArgumentNullExceptionHelper.ThrowIfNull(dtoAttributePicker, nameof(dtoAttributePicker));

        if (inputDto == null)
            return this;
        var valueResolver = resolver ?? (s => s);
        T? resolvedInputValue = valueResolver(dtoAttributePicker(inputDto));
        T? originalValue = default(T);
        bool doUpdate = false;
        if (entityOriginal == null)
        {
            doUpdate = true;
        }
        else
        {
            originalValue = entityAttributePicker.Compile()(entityOriginal);
            doUpdate = resolvedInputValue == null
                ? originalValue != null
                : (originalValue is null || !resolvedInputValue.Equals(originalValue));
        }

        if (doUpdate)
        {
            var setter = SetValueAction(entityAttributePicker);
            setter(entityDelta, resolvedInputValue);
            hasChanges = true;
        }

        return this;
    }

    public XrmDeltaMapper<TE, TI> MapAttribute<TEV, TIV>(Expression<Func<TE, TEV?>> entityAttributePicker, Func<TI, TIV?> dtoAttributePicker, Func<TIV?, TEV?> resolver)
        where TEV : struct, Enum
        where TIV : struct, Enum
    {
        ArgumentNullExceptionHelper.ThrowIfNull(entityAttributePicker, nameof(entityAttributePicker));
        ArgumentNullExceptionHelper.ThrowIfNull(dtoAttributePicker, nameof(dtoAttributePicker));
        ArgumentNullExceptionHelper.ThrowIfNull(resolver, nameof(resolver));

        if (inputDto == null)
            return this;
        var valueResolver = resolver;
        TEV? resolvedInputValue = valueResolver(dtoAttributePicker(inputDto));
        TEV? originalValue = null;
        bool doUpdate = false;
        if (entityOriginal == null)
        {
            doUpdate = true;
        }
        else
        {
            originalValue = entityAttributePicker.Compile()(entityOriginal);
            doUpdate = resolvedInputValue == null
                ? originalValue != null
                : (originalValue is null || !resolvedInputValue.Equals(originalValue));
        }

        if (doUpdate)
        {
            var setter = SetValueAction(entityAttributePicker);
            setter(entityDelta, resolvedInputValue);
            hasChanges = true;
        }

        return this;
    }

    public XrmDeltaMapper<TE, TI> MapAttribute(Expression<Func<TE, DateTime?>> entityAttributePicker, Func<TI, DateTime?> dtoAttributePicker, Func<DateTime?, DateTime?>? resolver = null)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(entityAttributePicker, nameof(entityAttributePicker));
        ArgumentNullExceptionHelper.ThrowIfNull(dtoAttributePicker, nameof(dtoAttributePicker));

        if (inputDto == null)
            return this;
        var valueResolver = resolver ?? (s => s);
        DateTime? resolvedInputValue = valueResolver(dtoAttributePicker(inputDto));
        DateTime? originalValue = null;
        bool doUpdate = false;
        if (entityOriginal == null)
        {
            doUpdate = true;
        }
        else
        {
            originalValue = entityAttributePicker.Compile()(entityOriginal);
            doUpdate = resolvedInputValue == null
                ? originalValue != null
                : (originalValue is null || resolvedInputValue != originalValue);
        }

        if (doUpdate)
        {
            var setter = SetValueAction(entityAttributePicker);
            setter(entityDelta, resolvedInputValue);
            hasChanges = true;
        }

        return this;
    }

    public XrmDeltaMapper<TE, TI> MapAttribute<T>(Expression<Func<TE, EntityReference?>> entityAttributePicker, Func<TI, T?> dtoAttributePicker, Func<T?, EntityReference?> resolver)
    {
        ArgumentNullExceptionHelper.ThrowIfNull(entityAttributePicker, nameof(entityAttributePicker));
        ArgumentNullExceptionHelper.ThrowIfNull(dtoAttributePicker, nameof(dtoAttributePicker));
        ArgumentNullExceptionHelper.ThrowIfNull(resolver, nameof(resolver));

        if (inputDto is null)
            return this;
        var valueResolver = resolver;
        EntityReference? resolvedInputValue = valueResolver(dtoAttributePicker(inputDto));
        EntityReference? originalValue = null;
        bool doUpdate = false;
        if (entityOriginal is null)
        {
            doUpdate = true;
        }
        else
        {
            originalValue = entityAttributePicker.Compile()(entityOriginal);
            doUpdate = resolvedInputValue is null
                ? originalValue is not null
                : (originalValue is null || resolvedInputValue.Id != originalValue.Id);
        }

        if (doUpdate)
        {
            var setter = SetValueAction(entityAttributePicker);
            setter(entityDelta, resolvedInputValue);
            hasChanges = true;
        }

        return this;
    }

    private static Action<TEntity, TValue?> SetValueAction<TEntity, TValue>(
        Expression<Func<TEntity, TValue>> attributePicker)
    {
        if (attributePicker.Body is not MemberExpression memberExpression)
            throw new ArgumentException("Expression must be a member access expression", nameof(attributePicker));

        if (memberExpression.Expression is not ParameterExpression entityParameter)
            throw new ArgumentException("Member expression must access a parameter", nameof(attributePicker));

        var valueParameter = Expression.Parameter(typeof(TValue));

        return Expression.Lambda<Action<TEntity, TValue?>>(
            Expression.Assign(memberExpression, valueParameter),
            entityParameter,
            valueParameter).Compile();
    }
}
