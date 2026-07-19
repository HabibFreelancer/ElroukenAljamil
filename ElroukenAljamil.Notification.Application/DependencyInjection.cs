using ElroukenAljamil.Notification.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ElroukenAljamil.Notification.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            services.AddScoped<INotificationOrchestrator, NotificationOrchestrator>();
            return services;
        }
    }
}
