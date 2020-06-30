using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.Sqs.Simple";
        #region ConfigureEndpoint

        var endpointConfiguration = new EndpointConfiguration("Samples.Sqs.Simple");
        var transport = endpointConfiguration.UseTransport<SqsTransport>();
        // hack
        transport.GetSettings().Set("NServiceBus.AmazonSQS.UnrestrictedDurationDelayedDeliveryQueueDelayTime", Convert.ToInt32(TimeSpan.FromMinutes(1).TotalSeconds));
        //transport.S3("bucketname", "my/key/prefix");

        #endregion
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        #region sendonly
        //TODO: uncomment to view a message in transit
        //endpointConfiguration.SendOnly();
        #endregion
        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);

        #region sends

        var dateTimeOffset = DateTimeOffset.UtcNow.AddMinutes(1);

        for (var i = 0; i < 100000; i++)
        {
            var options = new SendOptions();
            options.DoNotDeliverBefore(dateTimeOffset);
            options.RouteToThisEndpoint();
            var myMessage = new MyMessage();
            _ = endpointInstance.Send(myMessage, options).ContinueWith(t =>
            {
                return Console.Error.WriteAsync(".");
            });
        }


        #endregion
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}