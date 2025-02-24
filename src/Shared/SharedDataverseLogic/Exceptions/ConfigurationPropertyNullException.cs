namespace SharedDataverseLogic.Exceptions;

#pragma warning disable CA2237 // Mark ISerializable types with serializable
public class ConfigurationPropertyNullException : System.Exception
#pragma warning restore CA2237 // Mark ISerializable types with serializable
{
    public ConfigurationPropertyNullException()
    {
    }

    public ConfigurationPropertyNullException(string message)
        : base(message)
    {
    }

    public ConfigurationPropertyNullException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}