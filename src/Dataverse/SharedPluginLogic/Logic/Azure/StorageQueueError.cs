using System.Xml.Serialization;

namespace DataverseLogic.Azure;

[XmlType(TypeName = "Error")]
public class StorageQueueError
{
    public string? Code { get; set; }

    public string? Message { get; set; }

    public string? AuthenticationErrorDetail { get; set; }
}
