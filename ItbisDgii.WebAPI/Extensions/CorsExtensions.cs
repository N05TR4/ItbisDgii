namespace ItbisDgii.WebAPI.Extensions
{
    public static class CorsExtensions
    {
        public static void AddCorsExtensions(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost", builder =>
                builder.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:5251")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }
    }
}
