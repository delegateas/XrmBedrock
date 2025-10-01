using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace IntegrationTests;

/// <summary>
/// Enables the storage of serialized messages to a storage queue
/// Connects queue messages to the correct function, such that logic can be debugged
/// </summary>
public class MessageExecutor
{
    private List<AwaitingMessage> messages;

    public MessageExecutor()
    {
        messages = new List<AwaitingMessage>();

        // TODO: Add your Azure Function references here
    }

    public void StoreMessage(AwaitingMessage message) => messages.Add(message);

    public async Task SendMessages()
    {
        foreach (var message in messages)
        {
            // TODO: Add your queue message handling logic here
            await Task.CompletedTask;
        }

        messages = new List<AwaitingMessage>();
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