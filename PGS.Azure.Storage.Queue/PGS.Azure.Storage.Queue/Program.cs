using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using PGS.Azure.Storage.Queue.Configuration;

namespace PGS.Azure.Storage.Queue
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = ParseStorageAccountOptions();
            Console.WriteLine("Hello World!");
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
