using MailKit.Net.Smtp;
using MailKit.Security;
using Messaging.Infrastructure.Services.DeliveryProviders;
using Messaging.Infrastructure.Services.DeliveryProviders.EmailDelivery;
using Messaging.Infrastructure.Services.DeliveryProviders.SMSDelivery;
using Messaging.Infrastructure.Services.MessageQueue;
using Messaging.Infrastructure.Services.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Messaging.Infrastructure;

public static class DependencyInjection
{
    //public static IServiceCollection AddMessagingInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    //{
    //    services.Configure<MessageQueueSettings>(configuration.GetSection(MessageQueueSettings.SECTION_NAME));
    //    services.AddSingleton<RabbitConnectionProvider>();
    //    services.AddScoped<IMessageQueueService, MessageQueueService>();
    //    services.AddScoped<IMessageDeliveryService, MessageDeliveryService>();
    //    services.AddScoped<IEmailProviderService, EmailProviderService>();
    //    services.AddScoped<ISMSProviderService, SMSProviderService>();
    //    services.AddScoped<IMessagingRepository, MessagingRepository>();
    //    services.AddSingleton<IMessageConsumptionEventHandler, MessageConsumptionEventHandler>();
    //    services.AddDbContext<MessagingDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("Default")));
        
    //    return services;
    //}
    public static IServiceCollection AddMessagingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessageQueueSettings>(configuration.GetSection(MessageQueueSettings.SECTION_NAME));
        services.AddSingleton<RabbitConnectionProvider>();
        services.AddScoped<IMessageQueueService, MessageQueueService>();
        services.AddScoped<IMessageDeliveryService, MessageDeliveryService>();
    

        services.AddScoped<IMessagingRepository, MessagingRepository>();
        services.AddSingleton<IMessageConsumptionEventHandler, MessageConsumptionEventHandler>();
        services.AddDbContext<MessagingDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("Default")));

        services.AddEmailServices(configuration).AddSMSServices(configuration);

        return services;
    }

    private static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SECTION_NAME));

        services.AddScoped<IEmailProviderService, EmailProviderService>();
        services.AddSingleton(provider =>
        {
            var smtpSettings = provider.GetRequiredService<IOptions<EmailSettings>>().Value;

            var smtpClient = new SmtpClient();

            // Set up the SMTP client with the configured settings
            smtpClient.Connect(smtpSettings.SmtpServerAddress, smtpSettings.SmtpPort, SecureSocketOptions.StartTlsWhenAvailable);
            smtpClient.Authenticate(smtpSettings.SmtpUsername, smtpSettings.SmtpPassword); // maybe do this per request or once a while somehow . mailkit sends errors for identity change


            return smtpClient;
        });

        services.AddHostedService<SmtpClientHostedService>();

        return services;
    }

    private static IServiceCollection AddSMSServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<SMSSettings>(configuration.GetSection(SMSSettings.SECTION_NAME));

        services.AddScoped<ISMSProviderService, SMSProviderService>();

        services.AddHttpClient<ISMSHttpClientService, SMSHttpClientService>();

        //services.AddHttpClient<ISMSHttpClientService, ISMSHttpClientService>(client =>
        //{
        //    client.BaseAddress = new Uri(builder.Configuration["BaseUrl"]);
        //});
        //.AddPolicyHandler(GetRetryPolicy())
        //.AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

}
