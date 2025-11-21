namespace SharedContext.Dao.Exceptions;

#pragma warning disable CA2237 // Mark ISerializable types with serializable
public class DataverseInteractionException : Exception
#pragma warning restore CA2237 // Mark ISerializable types with serializable
{
    public DataverseInteractionException()
    {
    }

    public DataverseInteractionException(string message)
        : base(message)
    {
    }

    public DataverseInteractionException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
