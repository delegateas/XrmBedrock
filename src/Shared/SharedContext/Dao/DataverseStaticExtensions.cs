using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace SharedContext.Dao;

public static class DataverseStaticExtensions
{
    /// <summary>
    /// Extracts the attribute name of a lambda expression pointing to an attribute of entity T eg. for x => x.Name, "name" will be extracted
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expr"></param>
    /// <returns></returns>
    public static string StringOf<T>(this Expression<Func<T, object>> expr)
    {
        return StaticReflection.GetMemberName<T>(expr).ToLower();
    }

    public static string StringOf<T>(this Expression<Func<T, EntityReference>> expr)
    {
        return StaticReflection.GetMemberName<T>(expr).ToLower();
    }

    public static string LogicalName<T>() where T : Entity
    {
        return (Activator.CreateInstance<T>()).LogicalName;
    }

    public static EntityReference ToEntityReference<T>(this Guid guid) where T : Entity { return new EntityReference(typeof(T).Name.ToLower(), guid); }


    // This one could probably be in XrmExtensions instead
    public static List<ExecuteMultipleResponseItem> PerformAsBulk<T>(this IOrganizationService service, IEnumerable<T> requests, bool continueOnError = true, int chunkSize = 1000) where T : OrganizationRequest
    {
        var arr = requests.ToArray();
        var splitReqs = from i in Enumerable.Range(0, arr.Length)
                        group arr[i] by i / chunkSize;

        var resps = new List<ExecuteMultipleResponseItem>();
        foreach (var rs in splitReqs)
        {
            var req = new ExecuteMultipleRequest();
            req.Requests = new OrganizationRequestCollection();
            req.Requests.AddRange(rs);
            req.Settings = new ExecuteMultipleSettings();
            req.Settings.ContinueOnError = continueOnError;
            req.Settings.ReturnResponses = true;
            var resp = service.Execute(req) as ExecuteMultipleResponse;
            resps.AddRange(resp.Responses);
        }
        return resps;
    }

    /// <summary>
    /// Pseudo method to take the non-nullable value of a guid (which should never be null).
    /// This is to avoid null checks in the code, where stylecop ignores it.
    /// </summary>
    /// <param name="guid">Input</param>
    /// <returns>Output</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Guid GetRequiredValue(this Guid? guid)
    {
        if (guid == null)
        {
            throw new ArgumentException("Guid is null");
        }

        return guid.Value;
    }

    /// <summary>
    /// Takes a substring of the string input if it is longer than allowed by the specified attribute. 
    /// </summary>
    /// <typeparam name="T">Entity of the attribute</typeparam>
    /// <param name="stringValue">Value to truncate</param>
    /// <param name="attributeExpression">Must specify an attribute of type 'string' (text)</param>
    /// <returns></returns>
    public static string TruncateIfOutOfRange<T>(string stringValue, Expression<Func<T, object>> attributeExpression) where T : Entity
    {
        var maxLength = GetStringAttributeMaxLenth(attributeExpression);
        return stringValue.Length > maxLength ? stringValue.Substring(0, maxLength - 1) : stringValue;
    }

    /// <summary>
    /// Return the allowed lenght of the string attribute.
    /// </summary>
    /// <typeparam name="T">Entity of the attribute</typeparam>
    /// <param name="attributeExpression">Must specify an attribute of type 'string' (text)</param>
    /// <returns>Allowd max length</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static int GetStringAttributeMaxLenth<T>(Expression<Func<T, object>> attributeExpression) where T : Entity
    {
        // Get the attribute/porperty name
        var propertyName = StaticReflection.GetMemberName(attributeExpression);

        Type type = typeof(T);
        var property = type.GetProperty(propertyName);

        // Check if the property exists
        if (property == null)
        {
            throw new ArgumentException($"Property '{propertyName}' not found in type '{type.FullName}'");
        }

        // Get the MaxLength attribute
        MaxLengthAttribute maxLengthAttribute = property.GetCustomAttributes(typeof(MaxLengthAttribute), false)
                                                         .Cast<MaxLengthAttribute>()
                                                         .FirstOrDefault();

        // Check if the MaxLength attribute is found
        if (maxLengthAttribute == null)
        {
            throw new InvalidOperationException($"MaxLength attribute not found on property '{propertyName}'");
        }

        // Return the length value from the MaxLength attribute
        return maxLengthAttribute.Length;
    }

    #region Private classes

    /// <summary>
    /// http://joelabrahamsson.com/getting-property-and-method-names-using-static-reflection-in-c/
    /// </summary>
    private static class StaticReflection
    {
        public static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            return GetMemberName(expression.Body);
        }

        public static string GetMemberName<T>(Expression<Func<T, EntityReference>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            return GetMemberName(expression.Body);
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(
                    "The expression cannot be null.");
            }

            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression =
                    (MemberExpression)expression;
                return memberExpression.Member.Name;
            }

            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression =
                    (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMemberName(unaryExpression);
            }

            throw new ArgumentException("Invalid expression");
        }

        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression =
                    (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.Name;
            }

            return ((MemberExpression)unaryExpression.Operand)
                .Member.Name;
        }
    }

    #endregion
}
