using ClientApi.Handlers;
using CommonLib.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace ClientApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services
                .AddOptions()
                .Configure<ApiConfig>(hostContext.Configuration.GetSection("ApiConfig"));
                var config = new ApiConfig();
                hostContext.Configuration.GetSection("ApiConfig").Bind(config);
                services
                .AddSingleton<ICustomerHandler, CustomerHandler>()
                .AddSingleton<IOrderHandler, OrderHandler>();
                
            }).UseStartup<Startup>();
    }
}
