using Azure.Storage.Queues;
using Facets.Core.Common.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Facets.Infrastructure.NotificationServices;

public sealed class QueueService: IQueueService
{
    private readonly QueueServiceClient _queueServiceClient;

    public QueueService(QueueServiceClient queueServiceClient)
    {
        _queueServiceClient = queueServiceClient;
    }

    //public async Task Enqueue(string queueName, object message)
    //{
    //    var queueClient = _queueServiceClient.GetQueueClient(queueName);

    //    await queueClient.CreateIfNotExistsAsync();

    //    var jsonData = JsonConvert.SerializeObject(message);

    //    var jsonInBytes = Encoding.UTF8.GetBytes(jsonData);

    //    var emailAsBase64String = Convert.ToBase64String(jsonInBytes);

    //    await queueClient.SendMessageAsync(emailAsBase64String).ConfigureAwait(false);
    //}
    public async Task Enqueue(string queueName, object message)
    {
        try
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();

            // Serialize the message to JSON with custom settings
            string jsonData;
            try
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                };
                jsonData = JsonConvert.SerializeObject(message, Formatting.Indented, settings);
            }
            catch (JsonSerializationException jsonEx)
            {
                // Handle or log JSON serialization specific errors
                throw new InvalidOperationException("Error serializing the message to JSON.", jsonEx);
            }

            // Check if the message is within the size limit
            if (Encoding.UTF8.GetByteCount(jsonData) > 65536)
            {
                throw new InvalidOperationException("Message exceeds the maximum size of 64 KB.");
            }

            var jsonInBytes = Encoding.UTF8.GetBytes(jsonData);
            var emailAsBase64String = Convert.ToBase64String(jsonInBytes);
            await queueClient.SendMessageAsync(emailAsBase64String).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            // Example: Logger.LogError(ex, "Failed to enqueue message.");
            throw; // Re-throw or handle the exception based on your requirements
        }
    }
}
