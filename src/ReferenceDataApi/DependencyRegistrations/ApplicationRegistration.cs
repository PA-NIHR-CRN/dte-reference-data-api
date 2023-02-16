using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ReferenceDataApi.DependencyRegistrations
{
    public static class ApplicationRegistration
    {
        private const string ApplicationAssemblyName = "Application";

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.Load(ApplicationAssemblyName));
            
            return services;
        }
    }
}