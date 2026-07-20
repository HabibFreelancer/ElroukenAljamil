using ElroukenAljamil.BuildingBlocks.Security.DevBypass;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ElroukenAljamil.BuildingBlocks.Security.Extensions
{
    public static class DevAuthBypassExtensions
    {
        /// <summary>
        /// Enregistre les options DevUser depuis la configuration.
        /// À appeler dans builder.Services avant Build().
        /// </summary>
        public static IServiceCollection AddDevAuthBypass(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DevUserOptions>(
                configuration.GetSection(DevUserOptions.SectionName));
            return services;
        }

        /// <summary>
        /// Active le middleware de bypass JWT uniquement en Development.
        /// À placer AVANT app.UseAuthentication() et app.UseAuthorization().
        /// </summary>
        public static IApplicationBuilder UseDevAuthBypass(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseMiddleware<DevAuthBypassMiddleware>();

            return app;
        }
    }
}
