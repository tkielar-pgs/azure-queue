using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace PGS.Azure.Storage.Queue.Examples
{
    public class RandomProducer
    {
        private readonly CloudQueue _queue;
        private TimeSpan _maxMessageInterval;       
        private readonly Random _rand = new Random();

        public RandomProducer(CloudQueue queue, TimeSpan? maxMessageInterval = null)
        {
            _queue = queue;
            _maxMessageInterval = maxMessageInterval ?? TimeSpan.FromSeconds(10);
        }

        public async Task StartSendingMessages(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {                    
                    await SendMessage(cancellationToken);
                    await Task.Delay(TimeSpan.FromMilliseconds(_maxMessageInterval.TotalMilliseconds * _rand.NextDouble()), cancellationToken);
                }
                catch (TaskCanceledException)
                {                    
                }
            }
        }

        private Task SendMessage(CancellationToken cancellationToken)
        {
            var message = new CloudQueueMessage($"Message: {Guid.NewGuid()}");
            Console.WriteLine($"Producer sending - {message.AsString}");
            return _queue.AddMessageAsync(message, cancellationToken);
        }
    }
}