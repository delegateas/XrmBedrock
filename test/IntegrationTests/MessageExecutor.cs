using DataverseService.Foundation.Dao;
using EconomyAreaFunctionApp;
using Newtonsoft.Json;
using SharedDomain;
using SharedTest;
using System.Globalization;
using System.Text;

namespace IntegrationTests;

/// <summary>
/// Enables the storage of serialized messages to a storage queue
/// Connects queue messages to the correct function, such that logic can be debugged
/// </summary>
public class MessageExecutor
{
    private readonly List<AwaitingMessage> messages;
    private readonly CreateInvoices createInvoices;

    public MessageExecutor(IDataverseAccessObjectAsync adminDao)
    {
        messages = new List<AwaitingMessage>();

        createInvoices = new CreateInvoices(new SimpleLogger<CreateInvoices>(), new DataverseService.EconomyArea.DataverseInvoiceService(adminDao));
    }

    public void StoreMessage(AwaitingMessage message) => messages.Add(message);

    public async Task SendMessages()
    {
        foreach (var message in messages)
        {
            switch (message.QueueName)
            {
                case QueueNames.CreateInvoicesQueue:
                {
                    await createInvoices.Run(GetMessage<SharedDomain.EconomyArea.CreateInvoicesMessage>(message.SerializedMessage));
                    break;
                }
            }
        }
    }

    protected static T GetMessage<T>(string serializedMessage)
    {
        ArgumentNullException.ThrowIfNull(serializedMessage);

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
