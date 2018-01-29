using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using PGS.Azure.Storage.Queue.Configuration;
using PGS.Azure.Storage.Queue.Examples;

namespace PGS.Azure.Storage.Queue
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = ParseStorageAccountOptions();
            CloudQueue queue = GetQueue(options).GetAwaiter().GetResult();

            try
            {
                var example = new ProducerAnd2SimpleConsumersExample(queue);

                Console.WriteLine("Press CTRL+C to stop sending messages . . .");
                CancellationToken producerCancellationToken = GetConsoleCancellationToken();
                CancellationToken consumersCancellationToken = GetConsoleCancellationToken(producerCancellationToken);
                producerCancellationToken.Register(() => Console.WriteLine("Press CTRL+C to stop consumers . . ."));

                example.Run(producerCancellationToken, consumersCancellationToken).GetAwaiter().GetResult();                

                Console.WriteLine("Press any key to delete queue . . .");
                Console.ReadKey();
            }
            catch (StorageException exception)
            {
                Console.Error.WriteLine(exception.RequestInformation.ExtendedErrorInformation.ErrorMessage);
                throw;
            }
            finally
            {
                queue.DeleteIfExists();
            }
        }

        private static async Task<CloudQueue> GetQueue(StorageAccountOptions options)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(options.Name, options.Key), true);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference($"{options.QueueName}{new Random().Next()}");
            Console.WriteLine($"Using queue '{queue.Name}'");
            await queue.CreateIfNotExistsAsync();
            return queue;
        }

        private static StorageAccountOptions ParseStorageAccountOptions()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build();

            var options = new StorageAccountOptions();
            configuration.Bind("StorageAccount", options);
            return options;
        }

        private static CancellationToken GetConsoleCancellationToken(CancellationToken? dependency = null)
        {
            var source = new CancellationTokenSource();
            void SetUpEventHandler()
            {                
                Console.CancelKeyPress += (sender, args) =>
                {
                    args.Cancel = true;
                    if (dependency?.IsCancellationRequested ?? true)
                    {                    
                        source.Cancel();
                    }
                };
            }

            if (dependency != null)
            {
                dependency.Value.Register(SetUpEventHandler);
            }
            else
            {
                SetUpEventHandler();
            }

            return source.Token;
        }
    }
}
