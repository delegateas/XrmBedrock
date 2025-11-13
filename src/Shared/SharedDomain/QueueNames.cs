namespace SharedDomain;

// Used to align queue names across Dataverse and Azure
public static class QueueNames
{
    public const string CreateInvoicesQueue = "createinvoicesqueue";

    public static readonly IReadOnlyList<string> AllQueues = [
        CreateInvoicesQueue
    ];
}