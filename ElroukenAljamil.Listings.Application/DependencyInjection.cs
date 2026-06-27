using FluentValidation;
using ElroukenAljamil.Listings.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace ElroukenAljamil.Listings.Application
{
    /// <summary>
    /// Extension pour enregistrer les services de la couche Application.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;


            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddValidatorsFromAssembly(assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


            return services;
        }
    }

}
