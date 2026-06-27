using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using ElroukenAljamil.Listings.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.SqlServer;
using ElroukenAljamil.Common.Interfaces;


namespace ElroukenAljamil.Listings.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration SQL Server
            services.AddDbContext<ListingsDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("ListingsDb"),
                    b => b.MigrationsAssembly(typeof(ListingsDbContext).Assembly.FullName)));

            // Enregistrement des repositories
            services.AddScoped<IListingRepository, ListingRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ListingsDbContext>());


            return services;
        }
    }

}
