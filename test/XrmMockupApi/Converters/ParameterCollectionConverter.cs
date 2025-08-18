using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace XrmMockupApi.Converters;

public class ParameterCollectionConverter : JsonConverter<ParameterCollection>
{
    public override ParameterCollection ReadJson(JsonReader reader, Type objectType, ParameterCollection? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
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

        // Check if this is an EntityReference
        if (jObject.ContainsKey("LogicalName") && jObject.ContainsKey("Id"))
        {
            var logicalName = jObject["LogicalName"]?.Value<string>();
            var id = jObject["Id"]?.Value<Guid>();
            var name = jObject["Name"]?.Value<string>();

            if (!string.IsNullOrEmpty(logicalName) && id.HasValue)
            {
                return new EntityReference(logicalName, id.Value) { Name = name };
            }
        }

        // Check if this is an Entity
        if (jObject.ContainsKey("LogicalName") && jObject.ContainsKey("Attributes"))
        {
            var logicalName = jObject["LogicalName"]?.Value<string>();
            if (!string.IsNullOrEmpty(logicalName))
            {
                var entity = new Entity(logicalName);

                if (jObject["Id"] != null)
                {
                    entity.Id = jObject["Id"]!.Value<Guid>();
                }

                var attributes = jObject["Attributes"] as JObject;
                if (attributes != null)
                {
                    foreach (var attr in attributes.Properties())
                    {
                        entity[attr.Name] = DeserializeValue(attr.Value, serializer);
                    }
                }

                return entity;
            }
        }

        // Check if this is an OptionSetValue
        if (jObject.ContainsKey("Value") && jObject.Count == 1)
        {
            var value = jObject["Value"]?.Value<int>();
            if (value.HasValue)
            {
                return new OptionSetValue(value.Value);
            }
        }

        // Check if this is a Money value
        if (jObject.ContainsKey("Value") && jObject.Count == 1)
        {
            var value = jObject["Value"]?.Value<decimal>();
            if (value.HasValue)
            {
                return new Money(value.Value);
            }
        }

        // For other objects, try to deserialize using the default serializer
        try
        {
            return jObject.ToObject<object>(serializer);
        }
        catch
        {
            // If that fails, return as a dictionary
            var dict = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var property in jObject.Properties())
            {
                dict[property.Name] = DeserializeValue(property.Value, serializer);
            }

            return dict;
        }
    }

    private static object DeserializeArray(JToken token, JsonSerializer serializer)
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
                break;

            case Entity entity:
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
                break;

            case OptionSetValue optionSet:
                writer.WriteStartObject();
                writer.WritePropertyName("Value");
                writer.WriteValue(optionSet.Value);
                writer.WriteEndObject();
                break;

            case Money money:
                writer.WriteStartObject();
                writer.WritePropertyName("Value");
                writer.WriteValue(money.Value);
                writer.WriteEndObject();
                break;

            default:
                serializer.Serialize(writer, value);
                break;
        }
    }
}