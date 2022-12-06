using Microsoft.Extensions.DependencyInjection;
using Pat.Sender.Correlation;
using Pat.Sender.MessageGeneration;
using Pat.Sender.NetCoreLog;

namespace Pat.Sender.NetCoreDependencyResolution
{
    public static class ServiceCollectionAddPatSender
    {
        public static IServiceCollection AddPatSender(this IServiceCollection services, PatSenderSettings senderSettings)
        {
            services.AddSingleton(senderSettings);
            services.AddSingleton<ICorrelationIdProvider, NewCorrelationIdProvider>();
            services.AddPatSenderNetCoreLogAdapter();
            services.AddSingleton<MessageProperties>();
            services.AddTransient<IMessageSender, MessageSender>();
            services.AddSingleton<IMessageGenerator, MessageGenerator>();

            services.AddTransient<IMessagePublisher>(
               p => new MessagePublisher(
                   p.GetRequiredService<IMessageSender>(),
                   p.GetRequiredService<IMessageGenerator>(),
                   new MessageProperties(p.GetRequiredService<ICorrelationIdProvider>())));

            services.AddSingleton(senderSettings);

            return services;
        }
    }
}
