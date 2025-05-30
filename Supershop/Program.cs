using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Supershop.Data;
using System;

namespace Supershop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            RunSeeding(host);
            host.Run();
        }

        private static void RunSeeding(IHost host)
        {
            try
            {
                var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetService<SeedDb>();
                    seeder.SeedAsync().Wait();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw; // Re-throw to ensure the application fails visibly during debugging
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
