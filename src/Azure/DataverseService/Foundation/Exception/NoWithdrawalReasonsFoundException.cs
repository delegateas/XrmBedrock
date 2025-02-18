namespace DataverseService.Foundation.Exception;

public class NoWithdrawalReasonsFoundException : System.Exception
{
    public NoWithdrawalReasonsFoundException()
    {
    }

    public NoWithdrawalReasonsFoundException(string message)
        : base(message)
    {
    }

    public NoWithdrawalReasonsFoundException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}