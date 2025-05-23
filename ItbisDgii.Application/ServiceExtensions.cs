using FluentValidation;
using ItbisDgii.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ItbisDgii.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => 
                                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationsBehaviors<,>));
            

            
        }
    }
}
