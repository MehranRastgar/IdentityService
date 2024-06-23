using System.Text.RegularExpressions;
using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmsEvents;
using MailEvents;


public static class MassTransitConfig
{
    public static void ConfigureMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("hoopoe");
                    h.Password("geDteDd0Ltg2135FJYQ6rjNYHYkGQa70");
                });

                cfg.Message<SmsEvent>(e =>
                {
                    e.SetEntityName("SmsEvents:OtpMessage");
                });

                cfg.Message<MailMessage>(e =>
                {
                    e.SetEntityName("MailEvents:MailMessage");
                });

                cfg.ConfigureEndpoints(ctx);

            });

        });
  
    }
}