using Microsoft.Xrm.Sdk;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace XrmMockupApi.Converters.SystemTextJson;

public class OrganizationResponseJsonConverter : JsonConverter<OrganizationResponse>
{
    public override OrganizationResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Reading OrganizationResponse is not supported");
    }

    public override void Write(Utf8JsonWriter writer, OrganizationResponse value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("ResponseName", value.ResponseName);

        writer.WritePropertyName("Results");
        var paramConverter = new ParameterCollectionJsonConverter();
        paramConverter.Write(writer, value.Results, options);

        writer.WriteEndObject();
    }
}