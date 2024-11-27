namespace DataverseLogic.Azure
{
    public record AzureConfig(
        string StorageAccountToken,
        Uri StorageAccountUrl
    );
}