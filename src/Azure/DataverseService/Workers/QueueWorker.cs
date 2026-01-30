using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataverseService.Workers;

public abstract class QueueWorker<TMessage>(
    IServiceScopeFactory scopeFactory,
    QueueClient queueClient,
    ILogger logger) : BackgroundService
    where TMessage : class
{
    private readonly TimeSpan pollingInterval = TimeSpan.FromSeconds(1);
    private readonly TimeSpan errorDelay = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                QueueMessage? message = await queueClient.ReceiveMessageAsync(cancellationToken: stoppingToken);

                if (message?.Body is not null)
                {
                    var payload = JsonSerializer.Deserialize<TMessage>(message.Body.ToString());

                    if (payload is null)
                    {
                        logger.LogWarning("Received null message payload for queue {QueueName}", queueClient.Name);
                    }
                    else
                    {
                        using var scope = scopeFactory.CreateScope();
                        await ProcessMessageAsync(payload, scope.ServiceProvider, stoppingToken);
                    }

                    await queueClient.DeleteMessageAsync(
                        message.MessageId,
                        message.PopReceipt,
                        stoppingToken);
                }
                else
                {
                    await Task.Delay(pollingInterval, stoppingToken);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Error processing queue message for queue {QueueName}", queueClient.Name);
                await Task.Delay(errorDelay, stoppingToken);
            }
        }
    }

    protected abstract Task ProcessMessageAsync(TMessage message, IServiceProvider scopedServices, CancellationToken cancellationToken);
}
