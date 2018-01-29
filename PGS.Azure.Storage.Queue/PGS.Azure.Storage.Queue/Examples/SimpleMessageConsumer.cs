using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace PGS.Azure.Storage.Queue.Examples
{
    public class SimpleMessageConsumer
    {
        private readonly CloudQueue _queue;
        private readonly string _consumerName;

        public SimpleMessageConsumer(CloudQueue queue, string consumerName)
        {
            _queue = queue;
            _consumerName = consumerName;
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