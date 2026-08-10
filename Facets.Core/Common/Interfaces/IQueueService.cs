namespace Facets.Core.Common.Interfaces;

public interface IQueueService
{
    Task Enqueue(string queueName, object message);
}
