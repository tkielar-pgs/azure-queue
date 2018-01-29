﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using PGS.Azure.Storage.Queue.Configuration;

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
                Console.WriteLine("Hello World!");
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
    }
}
