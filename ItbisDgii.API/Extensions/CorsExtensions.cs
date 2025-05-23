namespace ItbisDgii.API.Extensions
{
    public static class CorsExtensions
    {
        public static void AddCorsExtensions(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", builder =>
                builder.WithOrigins("http://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }
    }
}
