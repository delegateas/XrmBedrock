using Microsoft.Xrm.Sdk;
using SharedDataverseLogic;
#if NET462
using System.Net.Http;
#endif
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DataverseLogic.Azure;

public class AzureService
{
#pragma warning disable S1450 // Private fields only used as local variables in methods should become local variables
    private readonly AzureConfig azureConfig;
    private readonly IExtendedTracingService tracingService;
    private readonly IManagedIdentityService managedIdentityService;
#pragma warning restore S1450 // Private fields only used as local variables in methods should become local variables
    private static readonly string[] StorageScopes = new string[] { "https://storage.azure.com/.default" };

    public AzureService(AzureConfig azureConfig, IExtendedTracingService tracingService, IManagedIdentityService managedIdentityService)
    {
        this.azureConfig = azureConfig;
        this.tracingService = tracingService;
        this.managedIdentityService = managedIdentityService;
    }

    // Generic way of sending messages to the storage queue
#pragma warning disable S1144 // Unused private types or members should be removed
    protected void SendStorageQueueMessage(string queueName, string message)
#pragma warning restore S1144 // Unused private types or members should be removed
    {
        var connectionString = $"{azureConfig.StorageAccountUrl}{queueName}/messages";
        try
        {
            QueueMessageAsync(message, new Uri(connectionString)).Wait();
        }
        catch (Exception e)
        {
            throw new InvalidPluginExecutionException(e.InnerException?.Message);
        }
    }

    // Http call to send the message to the storage queue
    private async Task QueueMessageAsync(
        string message,
        Uri connectionString)
    {
        var token = managedIdentityService.AcquireToken(StorageScopes);
        var payload = $"<QueueMessage><MessageText>{Convert.ToBase64String(Encoding.UTF8.GetBytes(message))}</MessageText></QueueMessage>";
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        client.DefaultRequestHeaders.Add("x-ms-version", "2024-11-04");
        using var content = new StringContent(payload);
        var resp = await client.PostAsync(connectionString, content);

        if (resp.StatusCode == System.Net.HttpStatusCode.OK ||
            resp.StatusCode == System.Net.HttpStatusCode.Created)
        {
            return;
        }

        var serializer = new XmlSerializer(typeof(StorageQueueError));
        var respBody = await resp.Content.ReadAsStreamAsync();
        using var reader = XmlReader.Create(respBody);
        var error = serializer.Deserialize(reader) as StorageQueueError;

        reader.Dispose();
        throw new InvalidPluginExecutionException($"{error?.Code}\n{error?.Message}\n{error?.AuthenticationErrorDetail}");
    }

    // Current best method for calling an Azure Function with a HTTP Trigger. Can be improved to not require a url with a function code in it.
#pragma warning disable S1144 // Unused private types or members should be removed
    private async Task<string> CallHttpTrigger(Uri url, string payload)
#pragma warning restore S1144 // Unused private types or members should be removed
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders
            .Accept
            .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        tracingService.Trace("Sending request to http trigger with application json");
        var resp = await client.PostAsync(url, content);

        tracingService.Trace($"Response {resp.StatusCode}");
        if (resp.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new InvalidPluginExecutionException($"Failed to call target. Status code: {resp.StatusCode}");
        }

        return await resp.Content.ReadAsStringAsync();
    }
}