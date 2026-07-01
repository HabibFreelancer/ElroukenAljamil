using System.Reflection;
using ElroukenAljamil.Listings.Application.Behaviors;
using FluentValidation;
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
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
            services.AddAutoMapper(assembly);
            services.AddValidatorsFromAssembly(assembly);

            // Pipeline behavior pour la validation automatique
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }

}
