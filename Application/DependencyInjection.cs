using Application.Contexts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration conf)
        {
            var db = (conf.GetSection("Database").Value ?? "SQL").ToUpper();
            switch (db)
            {
                case "SQL":
                    {
                        var defaultConnectionString = conf.GetConnectionString("SqlConnection");
                        services.AddDbContextPool<AppDbContext>(opt =>
                        {
                            //opt.UseSqlServer(defaultConnectionString, provider =>
                            //{
                            //    //provider.CommandTimeout(180);
                            //});
                        });
                    }
                    break;                
            }

            var assembly = typeof(DependencyInjection).Assembly;

            services.AddValidatorsFromAssembly(assembly);

           
            return services;
        }
    }
}
