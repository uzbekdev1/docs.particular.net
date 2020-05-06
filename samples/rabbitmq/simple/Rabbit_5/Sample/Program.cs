using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.RabbitMQ.Simple";
        #region ConfigureRabbit
        var endpointConfiguration = new EndpointConfiguration("Samples.MSMQ.Simple");
        var transport = endpointConfiguration.UseTransport<MsmqTransport>();
        //transport.UseConventionalRoutingTopology();
        //transport.ConnectionString("host=localhost");
        #endregion

        LogManager.Use<NoOpLoggerDefintion>();

        endpointConfiguration.Recoverability().AddUnrecoverableException<InvalidOperationException>();
        endpointConfiguration.LimitMessageProcessingConcurrencyTo(50);
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        for (var i = 0; i < 100000; i++)
        {
            _ = Task.Run(() => Send(endpointInstance));
        }
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    static Task Send(IEndpointInstance endpointInstance)
    {
        var options = new SendOptions();
        options.SetHeader("NServiceBus.ExceptionInfo.ExceptionType", "System.InvalidOperationException");
        options.SetHeader("NServiceBus.ExceptionInfo.Message", "Operation is not valid due to the current state of the object.");
        options.SetHeader("NServiceBus.ExceptionInfo.Source", "Sample");
        options.SetHeader("NServiceBus.ExceptionInfo.StackTrace", "System.InvalidOperationException: Operation is not valid due to the current state of the object.");
        options.SetHeader("NServiceBus.TimeOfFailure", DateTimeExtensions.ToWireFormattedString(DateTime.UtcNow));
        options.SetHeader("NServiceBus.FailedQ", "Samples.MSMQ.Simple");
        options.SetHeader("NServiceBus.ProcessingEndpoint", "Samples.MSMQ.Simple");
        options.SetDestination("error");

        return endpointInstance.Send(new MyMessage(), options);
    }

    public class NoOpLoggerDefintion : LoggingFactoryDefinition
    {
        protected override ILoggerFactory GetLoggingFactory()
        {
            return new NoOpLoggerFactory();
        }
    }

    public class NoOpLoggerFactory : ILoggerFactory
    {
        public ILog GetLogger(Type type)
        {
            return new NoOpLogger();
        }

        public ILog GetLogger(string name)
        {
            return new NoOpLogger();
        }

        class NoOpLogger : ILog
        {
            public void Debug(string message)
            {
            }

            public void Debug(string message, Exception exception)
            {
            }

            public void DebugFormat(string format, params object[] args)
            {
            }

            public void Info(string message)
            {
            }

            public void Info(string message, Exception exception)
            {
            }

            public void InfoFormat(string format, params object[] args)
            {
            }

            public void Warn(string message)
            {
            }

            public void Warn(string message, Exception exception)
            {
            }

            public void WarnFormat(string format, params object[] args)
            {
            }

            public void Error(string message)
            {
            }

            public void Error(string message, Exception exception)
            {
            }

            public void ErrorFormat(string format, params object[] args)
            {
            }

            public void Fatal(string message)
            {
            }

            public void Fatal(string message, Exception exception)
            {
            }

            public void FatalFormat(string format, params object[] args)
            {
            }

            public bool IsDebugEnabled { get; }
            public bool IsInfoEnabled { get; }
            public bool IsWarnEnabled { get; }
            public bool IsErrorEnabled { get; }
            public bool IsFatalEnabled { get; }
        }
    }
}