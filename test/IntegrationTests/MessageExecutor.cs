using DataverseService;
using DataverseService.EconomyArea;
using DataverseService.Foundation.Dao;
using EconomyAreaFunctionApp;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedDomain;
using SharedDomain.EconomyArea;
using System.Globalization;
using System.Text;

namespace IntegrationTests;

public class MessageExecutor
{
    private List<AwaitingMessage> messages;

    private CreateInvoices CreateInvoicesFunction { get; }

    public MessageExecutor(IDataverseAccessObjectAsync adminDao)
    {
        messages = new List<AwaitingMessage>();
        CreateInvoicesFunction = new CreateInvoices(Substitute.For<ILogger<CreateInvoices>>(), new DataverseInvoiceService(adminDao, new DataverseCustomApiService(adminDao)));
    }

    public void StoreMessage(AwaitingMessage message) => messages.Add(message);

    public async Task SendMessages()
    {
        foreach (var message in messages)
        {
            switch (message.QueueName)
            {
                case QueueNames.CreateInvoicesQueue:
                    await CreateInvoicesFunction.Run(GetMessage<CreateInvoicesMessage>(message.SerializedMessage));
                    break;
            }
        }

        messages = new List<AwaitingMessage>();
    }

    private static T GetMessage<T>(string serializedMessage)
    {
        var message =
            JsonConvert.DeserializeObject<T>(
               Encoding.UTF8.GetString(
                    Convert.FromBase64String(
                        serializedMessage
                        .Replace("<QueueMessage><MessageText>", string.Empty, true, CultureInfo.InvariantCulture)
                        .Replace("</MessageText></QueueMessage>", string.Empty, true, CultureInfo.InvariantCulture))));

        return message ?? throw new InvalidOperationException("Incorrect message received");
    }
}