using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples-Azure-StoragePersistence-Server";

        #region config

         var endpointConfiguration = new EndpointConfiguration("Samples-Azure-StoragePersistence-Server");

        // var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
        // persistence.ConnectionString("UseDevelopmentStorage=true");
        // endpointConfiguration.UsePersistence<AzureStoragePersistence, StorageType.Sagas>()
        //     .AssumeSecondaryIndicesExist();

        var persistence = endpointConfiguration.UsePersistence<CosmosDbPersistence>();
        persistence.EnableMigrationMode();
        persistence.CosmosClient(new CosmosClient("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="));
        persistence.DatabaseName("CosmosDBPersistence");
        persistence.DefaultContainer("OrderSagaData", "/id");

        var p2 = endpointConfiguration.UsePersistence<AzureStoragePersistence, StorageType.Subscriptions>();
        p2.ConnectionString("UseDevelopmentStorage=true");

        #endregion

        endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");
        var transport = endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
        transport.ConnectionString("UseDevelopmentStorage=true");
        transport.DelayedDelivery().DisableTimeoutManager();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}