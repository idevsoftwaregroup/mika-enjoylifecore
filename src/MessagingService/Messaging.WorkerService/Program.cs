using Messaging.Infrastructure;
using Messaging.Infrastructure.Services.MessageQueue;
using Messaging.WorkerService;
using System.Runtime;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext ,services) =>
    {
        services.AddHostedService<Worker>();
        services.AddMessagingInfrastructure(hostContext.Configuration);
    })
    .Build();

await host.RunAsync();
