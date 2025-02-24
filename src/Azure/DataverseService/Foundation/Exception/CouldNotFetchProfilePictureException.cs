namespace DataverseService.Foundation.Exception;

public class CouldNotFetchProfilePictureException : System.Exception
{
    public CouldNotFetchProfilePictureException()
    {
    }

    public CouldNotFetchProfilePictureException(string message)
        : base(message)
    {
    }

    public CouldNotFetchProfilePictureException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}