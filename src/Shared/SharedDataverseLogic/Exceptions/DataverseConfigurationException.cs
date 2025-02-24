namespace SharedDataverseLogic.Exceptions;

#pragma warning disable CA2237 // Mark ISerializable types with serializable
public class DataverseConfigurationException : System.Exception
#pragma warning restore CA2237 // Mark ISerializable types with serializable
{
    public DataverseConfigurationException()
    {
    }

    public DataverseConfigurationException(string message)
        : base(message)
    {
    }

    public DataverseConfigurationException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}