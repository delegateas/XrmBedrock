using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace XrmMockupApi.Converters;

public class ParameterCollectionConverter : JsonConverter<ParameterCollection>
{
    public override ParameterCollection ReadJson(JsonReader reader, Type objectType, ParameterCollection? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(reader);
        var parameters = new ParameterCollection();

        if (reader.TokenType == JsonToken.Null)
            return parameters;

        var jObject = JObject.Load(reader);

        foreach (var property in jObject.Properties())
        {
            var value = DeserializeValue(property.Value, serializer);
            parameters[property.Name] = value;
        }

        return parameters;
    }

    public override void WriteJson(JsonWriter writer, ParameterCollection? value, JsonSerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(serializer);
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();

        foreach (var entry in value)
        {
            writer.WritePropertyName(entry.Key);
            SerializeValue(writer, entry.Value, serializer);
        }

        writer.WriteEndObject();
    }

    private static object? DeserializeValue(JToken token, JsonSerializer serializer)
    {
        return token.Type switch
        {
            JTokenType.Null => null,
            JTokenType.Boolean => token.Value<bool>(),
            JTokenType.Integer => token.Value<int>(),
            JTokenType.Float => token.Value<double>(),
            JTokenType.String => token.Value<string>(),
            JTokenType.Date => token.Value<DateTime>(),
            JTokenType.Guid => token.Value<Guid>(),
            JTokenType.Object => DeserializeObject(token, serializer),
            JTokenType.Array => DeserializeArray(token, serializer),
            _ => token.Value<object>(),
        };
    }

    private static object? DeserializeObject(JToken token, JsonSerializer serializer)
    {
        var jObject = (JObject)token;

        return TryDeserializeEntityReference(jObject) ??
               TryDeserializeEntity(jObject, serializer) ??
               TryDeserializeOptionSetValue(jObject) ??
               TryDeserializeMoneyValue(jObject) ??
               TryDeserializeColumnSet(jObject) ??
               DeserializeGenericObject(jObject, serializer);
    }

    private static EntityReference? TryDeserializeEntityReference(JObject jObject)
    {
        if (!jObject.ContainsKey("LogicalName") || !jObject.ContainsKey("Id"))
            return null;

        var logicalName = jObject["LogicalName"]?.Value<string>();
        var idToken = jObject["Id"];
        var name = jObject["Name"]?.Value<string>();

        var id = ParseGuidToken(idToken);

        if (!string.IsNullOrEmpty(logicalName) && id.HasValue)
        {
            return new EntityReference(logicalName, id.Value) { Name = name };
        }

        return null;
    }

    private static Entity? TryDeserializeEntity(JObject jObject, JsonSerializer serializer)
    {
        if (!jObject.ContainsKey("LogicalName") || !jObject.ContainsKey("Attributes"))
            return null;

        var logicalName = jObject["LogicalName"]?.Value<string>();
        if (string.IsNullOrEmpty(logicalName))
            return null;

        var entity = new Entity(logicalName);

        var id = ParseGuidToken(jObject["Id"]);
        if (id.HasValue)
        {
            entity.Id = id.Value;
        }

        if (jObject["Attributes"] is JObject attributes)
        {
            foreach (var attr in attributes.Properties())
            {
                entity[attr.Name] = DeserializeValue(attr.Value, serializer);
            }
        }

        return entity;
    }

    private static OptionSetValue? TryDeserializeOptionSetValue(JObject jObject)
    {
        if (jObject.ContainsKey("Value") && jObject.Count == 1)
        {
            var value = jObject["Value"]?.Value<int>();
            if (value.HasValue)
            {
                return new OptionSetValue(value.Value);
            }
        }

        return null;
    }

    private static Money? TryDeserializeMoneyValue(JObject jObject)
    {
        if (jObject.ContainsKey("Value") && jObject.Count == 1)
        {
            var value = jObject["Value"]?.Value<decimal>();
            if (value.HasValue)
            {
                return new Money(value.Value);
            }
        }

        return null;
    }

    private static Microsoft.Xrm.Sdk.Query.ColumnSet? TryDeserializeColumnSet(JObject jObject)
    {
        if (!jObject.ContainsKey("AllColumns") && !jObject.ContainsKey("Columns"))
            return null;

        var columnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet();

        if (jObject["AllColumns"] != null)
        {
            columnSet.AllColumns = jObject["AllColumns"]!.Value<bool>();
        }

        if (jObject["Columns"] is JArray columnsArray)
        {
            foreach (var column in columnsArray)
            {
                var columnName = column.Value<string>();
                if (!string.IsNullOrEmpty(columnName))
                {
                    columnSet.Columns.Add(columnName);
                }
            }
        }

        return columnSet;
    }

    private static object DeserializeGenericObject(JObject jObject, JsonSerializer serializer)
    {
        try
        {
            return jObject.ToObject<object>(serializer) ?? new Dictionary<string, object?>(StringComparer.Ordinal);
        }
        catch
        {
            var dict = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var property in jObject.Properties())
            {
                dict[property.Name] = DeserializeValue(property.Value, serializer);
            }

            return dict;
        }
    }

    private static Guid? ParseGuidToken(JToken? idToken)
    {
        if (idToken == null)
            return null;

        return idToken.Type switch
        {
            JTokenType.String when Guid.TryParse(idToken.Value<string>(), out var parsedGuid) => parsedGuid,
            JTokenType.Guid => idToken.Value<Guid>(),
            _ => null,
        };
    }

    private static object?[] DeserializeArray(JToken token, JsonSerializer serializer)
    {
        var jArray = (JArray)token;
        var result = new List<object?>();

        foreach (var item in jArray)
        {
            result.Add(DeserializeValue(item, serializer));
        }

        return result.ToArray();
    }

    private static void SerializeValue(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        switch (value)
        {
            case EntityReference entityRef:
                SerializeEntityReference(writer, entityRef);
                break;
            case Entity entity:
                SerializeEntity(writer, entity, serializer);
                break;
            case OptionSetValue optionSet:
                SerializeOptionSetValue(writer, optionSet);
                break;
            case Money money:
                SerializeMoneyValue(writer, money);
                break;
            case Microsoft.Xrm.Sdk.Query.ColumnSet columnSet:
                SerializeColumnSet(writer, columnSet);
                break;
            default:
                serializer.Serialize(writer, value);
                break;
        }
    }

    private static void SerializeEntityReference(JsonWriter writer, EntityReference entityRef)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("LogicalName");
        writer.WriteValue(entityRef.LogicalName);
        writer.WritePropertyName("Id");
        writer.WriteValue(entityRef.Id);
        if (!string.IsNullOrEmpty(entityRef.Name))
        {
            writer.WritePropertyName("Name");
            writer.WriteValue(entityRef.Name);
        }

        writer.WriteEndObject();
    }

    private static void SerializeEntity(JsonWriter writer, Entity entity, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("LogicalName");
        writer.WriteValue(entity.LogicalName);
        writer.WritePropertyName("Id");
        writer.WriteValue(entity.Id);
        writer.WritePropertyName("Attributes");
        writer.WriteStartObject();
        foreach (var attr in entity.Attributes)
        {
            writer.WritePropertyName(attr.Key);
            SerializeValue(writer, attr.Value, serializer);
        }

        writer.WriteEndObject();
        writer.WriteEndObject();
    }

    private static void SerializeOptionSetValue(JsonWriter writer, OptionSetValue optionSet)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Value");
        writer.WriteValue(optionSet.Value);
        writer.WriteEndObject();
    }

    private static void SerializeMoneyValue(JsonWriter writer, Money money)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Value");
        writer.WriteValue(money.Value);
        writer.WriteEndObject();
    }

    private static void SerializeColumnSet(JsonWriter writer, Microsoft.Xrm.Sdk.Query.ColumnSet columnSet)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("AllColumns");
        writer.WriteValue(columnSet.AllColumns);
        writer.WritePropertyName("Columns");
        writer.WriteStartArray();
        foreach (var column in columnSet.Columns)
        {
            writer.WriteValue(column);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}