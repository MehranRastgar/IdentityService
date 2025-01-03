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
          cfg.Host("rabbitmq-ctcom", "/", h =>
              {
              h.Username("guest");
              h.Password("guest");
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