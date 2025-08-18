using Microsoft.Xrm.Sdk;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XrmMockupApi.Converters.SystemTextJson;

public class ParameterCollectionJsonConverter : JsonConverter<ParameterCollection>
{
    public override ParameterCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var parameters = new ParameterCollection();

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token");
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString()!;
                reader.Read();
                var value = DeserializeValue(ref reader, options);
                parameters[propertyName] = value;
            }
        }

        return parameters;
    }

    public override void Write(Utf8JsonWriter writer, ParameterCollection value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var entry in value)
        {
            writer.WritePropertyName(entry.Key);
            SerializeValue(writer, entry.Value, options);
        }

        writer.WriteEndObject();
    }

    private static object? DeserializeValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.True => true,
            JsonTokenType.False => false,
            JsonTokenType.Number => reader.TryGetInt32(out int intValue) ? intValue : reader.GetDouble(),
            JsonTokenType.String => reader.GetString(),
            JsonTokenType.StartObject => DeserializeObject(ref reader, options),
            JsonTokenType.StartArray => DeserializeArray(ref reader, options),
            _ => throw new JsonException($"Unexpected token type: {reader.TokenType}"),
        };
    }

    private static object? DeserializeObject(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var obj = new Dictionary<string, object?>(StringComparer.Ordinal);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString()!;
                reader.Read();
                obj[propertyName] = DeserializeValue(ref reader, options);
            }
        }

        return obj;
    }

    private static object DeserializeArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var list = new List<object?>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            list.Add(DeserializeValue(ref reader, options));
        }

        return list.ToArray();
    }

    private static void SerializeValue(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        switch (value)
        {
            case EntityReference entityRef:
                writer.WriteStartObject();
                writer.WriteString("LogicalName", entityRef.LogicalName);
                writer.WriteString("Id", entityRef.Id.ToString());
                if (!string.IsNullOrEmpty(entityRef.Name))
                {
                    writer.WriteString("Name", entityRef.Name);
                }

                writer.WriteEndObject();
                break;

            case Entity entity:
                writer.WriteStartObject();
                writer.WriteString("LogicalName", entity.LogicalName);
                writer.WriteString("Id", entity.Id.ToString());
                writer.WritePropertyName("Attributes");
                writer.WriteStartObject();
                foreach (var attr in entity.Attributes)
                {
                    writer.WritePropertyName(attr.Key);
                    SerializeValue(writer, attr.Value, options);
                }

                writer.WriteEndObject();
                writer.WriteEndObject();
                break;

            case OptionSetValue optionSet:
                writer.WriteStartObject();
                writer.WriteNumber("Value", optionSet.Value);
                writer.WriteEndObject();
                break;

            case Money money:
                writer.WriteStartObject();
                writer.WriteNumber("Value", money.Value);
                writer.WriteEndObject();
                break;

            case Microsoft.Xrm.Sdk.Query.ColumnSet columnSet:
                writer.WriteStartObject();
                writer.WriteBoolean("AllColumns", columnSet.AllColumns);
                writer.WritePropertyName("Columns");
                writer.WriteStartArray();
                foreach (var column in columnSet.Columns)
                {
                    writer.WriteStringValue(column);
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
                break;

            default:
                JsonSerializer.Serialize(writer, value, options);
                break;
        }
    }
}