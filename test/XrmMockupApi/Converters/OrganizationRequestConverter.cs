using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XrmMockupApi.Converters;

public class OrganizationRequestConverter : JsonConverter<OrganizationRequest>
{
    public override OrganizationRequest ReadJson(JsonReader reader, Type objectType, OrganizationRequest? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);

        var requestName = jObject["RequestName"]?.Value<string>();
        if (string.IsNullOrEmpty(requestName))
        {
            throw new JsonSerializationException("RequestName is required for OrganizationRequest deserialization");
        }

        var request = new OrganizationRequest(requestName);

        var parametersToken = jObject["Parameters"];
        if (parametersToken != null)
        {
            var parameterCollectionConverter = new ParameterCollectionConverter();
            using var paramReader = parametersToken.CreateReader();
            request.Parameters = parameterCollectionConverter.ReadJson(paramReader, typeof(ParameterCollection), null, false, serializer);
        }

        return request;
    }

    public override void WriteJson(JsonWriter writer, OrganizationRequest? value, JsonSerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();

        writer.WritePropertyName("RequestName");
        writer.WriteValue(value.RequestName);

        writer.WritePropertyName("Parameters");
        var parameterCollectionConverter = new ParameterCollectionConverter();
        parameterCollectionConverter.WriteJson(writer, value.Parameters, serializer);

        writer.WriteEndObject();
    }
}