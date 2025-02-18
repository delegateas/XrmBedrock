using Microsoft.Xrm.Sdk;

namespace DataverseLogic.Utility;

public static class EntityUtilityService
{
    /// <summary>
    /// Gets the property value of the entity from property selector.
    /// Throws an InvalidPluginExecutionException if the property value is null.
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TProp">Property type</typeparam>
    /// <param name="entity">entity</param>
    /// <param name="propertySelector">propertySelector</param>
    /// <returns>Property value</returns>
    /// <exception cref="ArgumentNullException">ArgumentNullException</exception>
    /// <exception cref="InvalidPluginExecutionException">InvalidPluginExecutionException</exception>
    public static TProp GetRequiredPropertyValue<T, TProp>(T entity, Func<T, TProp> propertySelector)
      where T : Entity
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
        }

        if (propertySelector == null)
        {
            throw new ArgumentNullException(nameof(propertySelector), "Property selector cannot be null.");
        }

        var propertyValue = propertySelector(entity);

        if (propertyValue == null)
        {
            throw new InvalidPluginExecutionException($"Property value is null of '{nameof(propertySelector)}'.");
        }

        return propertyValue;
    }
}