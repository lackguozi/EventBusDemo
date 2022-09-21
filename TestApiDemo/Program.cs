using EventBudDemo;
using Microsoft.AspNetCore.Builder;

namespace TestApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.ConfigureHttpService();
            // Add services to the container.
            builder.ConfigureExtraServices();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "TestService.WebAPI", Version = "v1" });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseCustomDefault();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileService.WebAPI v1"));
            app.UseEventBus();
            app.MapControllers();

            app.Run();
        }
    }
    public static class WebApplicationExtensions
    {
        public static void ConfigureHttpService(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            
            services.AddHttpClient<IHttpClientTest, HttpClientTest>();
            //var httpTest = 
        }

    }
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomDefault(this IApplicationBuilder builder)
        {
            try{
                var test = builder.ApplicationServices.GetRequiredService<IHttpClientTest>();
                test.Test(5);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return builder;
        }
    }
}