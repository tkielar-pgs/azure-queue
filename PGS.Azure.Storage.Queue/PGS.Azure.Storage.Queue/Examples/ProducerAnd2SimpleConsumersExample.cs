﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace PGS.Azure.Storage.Queue.Examples
{
    public class ProducerAnd2SimpleConsumersExample
    {
        private readonly CloudQueue _queue;

        public ProducerAnd2SimpleConsumersExample(CloudQueue queue) => _queue = queue;

        public Task Run(CancellationToken producerCancellationToken, CancellationToken consumersCancellationToken)
        {
            var consumers = new[]
            {
                new SimpleMessageConsumer(_queue, "Consumer 1"),
                new SimpleMessageConsumer(_queue, "Consumer 2")
            };
            var producer = new RandomProducer(_queue);

            IEnumerable<Task> consumersTasks = consumers.Select(consumer => consumer.StartConsumingMessages(consumersCancellationToken));
            Task producerTask = producer.StartSendingMessages(producerCancellationToken);

            return Task.WhenAll(consumersTasks.Concat(new[] {producerTask}));
        }
    }
}