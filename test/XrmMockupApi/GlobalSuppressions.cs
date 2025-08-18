using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Security", "CA2326:Deserializing JSON when using a TypeNameHandling value other than None can be insecure", Justification = "Required for Dataverse SDK type serialization")]
[assembly: SuppressMessage("Security", "SCS0028:TypeNameHandling is set to the other value than 'None'. It may lead to deserialization vulnerability", Justification = "Required for Dataverse SDK type serialization")]
[assembly: SuppressMessage("Design", "CA1031:Modify to catch a more specific allowed exception type, or rethrow the exception", Justification = "API endpoints need to catch all exceptions for proper error responses")]