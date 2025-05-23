using Microsoft.OpenApi.Models;
using System.Reflection;

namespace ItbisDgii.WebAPI.Extensions
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerExtensions(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                // Configuración básica del documento sin versionamiento
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    
                    Title = "ITBIS DGII API",
                    Description = "API para gestión de contribuyentes y comprobantes fiscales - Sistema DGII República Dominicana",
                    
                });

                // Configuración de seguridad JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando el esquema Bearer. \r\n\r\n" +
                                  "Ingrese 'Bearer' [espacio] y luego su token en el campo de texto a continuación.\r\n\r\n" +
                                  "Ejemplo: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                           
                        },
                        new string[] { }
                    }
                });

                
            });
        }
    }
}