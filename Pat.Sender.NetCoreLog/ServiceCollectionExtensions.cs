using Microsoft.Extensions.DependencyInjection;

namespace Pat.Sender.NetCoreLog
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the PatSenderNetCoreLogAdapter so pat sender logs will use Microsoft.Extensions.Logging
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to use.</param>
        /// <returns></returns>
        public static IServiceCollection AddPatSenderNetCoreLogAdapter(this IServiceCollection services)
            => services.AddTransient(typeof(IPatSenderLog<>), typeof(PatSenderNetCoreLogAdapter<>));
    }
}