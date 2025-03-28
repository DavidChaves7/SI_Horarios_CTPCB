using AutoMapper;
using Infrastructure.Common.Mappings;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var autoMapperConfig = new MapperConfiguration(c => {
                c.AddProfile(typeof(MappingProfile));
            });
            services.AddSingleton(autoMapperConfig.CreateMapper());
            //services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
