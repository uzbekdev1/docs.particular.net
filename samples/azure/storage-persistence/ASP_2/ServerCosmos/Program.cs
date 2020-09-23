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

        var persistence = endpointConfiguration.UsePersistence<CosmosDbPersistence>();
        persistence.EnableMigrationMode();
        persistence.CosmosClient(new CosmosClient("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="));
        persistence.DatabaseName("CosmosDBPersistence");
        persistence.DefaultContainer("OrderSagaData", "/id");

        #endregion

        endpointConfiguration.LimitMessageProcessingConcurrencyTo(100);

        endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");
        var transport = endpointConfiguration.UseTransport<LearningTransport>();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}