namespace SharedDataverseLogic.Exceptions;

#pragma warning disable CA2237 // Mark ISerializable types with serializable
public class ConfigurationNotFoundException : System.Exception
#pragma warning restore CA2237 // Mark ISerializable types with serializable
{
    public ConfigurationNotFoundException()
    {
    }

    public ConfigurationNotFoundException(string message)
        : base(message)
    {
    }

    public ConfigurationNotFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}