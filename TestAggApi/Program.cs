using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace TestAggApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Configuration.AddJsonFile("Ocelot.json", false, true);
            
            builder.Services.AddControllers();
            builder.Services.AddOcelot();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
            
            app.Use((context, next) =>
            {
                Console.WriteLine("sdsd");
                return next(context);
            });
            //app.UseOcelot().Wait();
            app.Run();
        }
    }
}