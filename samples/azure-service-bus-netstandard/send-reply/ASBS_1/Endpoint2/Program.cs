using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.ASBS.SendReply.Endpoint2";

        var endpointConfiguration = new EndpointConfiguration("Samples.ASBS.SendReply.Endpoint2");
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UnitOfWork().WrapHandlersInATransactionScope();

        endpointConfiguration.Pipeline.Register(new MyBehavior(), "dafdfds");

        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.Transactions(TransportTransactionMode.SendsAtomicWithReceive);

        var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Could not read the 'AzureServiceBus_ConnectionString' environment variable. Check the sample prerequisites.");
        }

        transport.ConnectionString(connectionString);

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();

        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
    class MyBehavior : Behavior<IIncomingPhysicalMessageContext>
    {
        public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
        {
            using (var txScope = new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await next();
            }
        }
    }
}