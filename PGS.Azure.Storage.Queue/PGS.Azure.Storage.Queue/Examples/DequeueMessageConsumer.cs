using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace PGS.Azure.Storage.Queue.Examples
{
    public class DequeueMessageConsumer
    {
        private readonly string _consumerName;
        private readonly CloudQueue _queue;

        public DequeueMessageConsumer(CloudQueue queue, string consumerName)
        {
            _consumerName = consumerName;
            _queue = queue;
        }

        public async Task StartConsumingMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    CloudQueueMessage message = await _queue.GetMessageAsync(cancellationToken);
                    if (message != null)
                    {
                        Console.WriteLine($"Consumer '{_consumerName}' received '{message.AsString}'");
                        await _queue.DeleteMessageAsync(message, cancellationToken);
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }
    }
}