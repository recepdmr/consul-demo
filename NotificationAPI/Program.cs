using System;
using Consul;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Winton.Extensions.Configuration.Consul;

namespace NotificationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(builder => { builder.AddJsonFile("appsettings.json", false, true); })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    string consulHost = context.Configuration["ConsulHost"];
                    string applicationName = context.HostingEnvironment.ApplicationName;
                    string environmentName = context.HostingEnvironment.EnvironmentName;

                    void ConsulConfig(ConsulClientConfiguration configuration)
                    {
                        configuration.Address = new Uri(consulHost);
                    }

                    builder.AddConsul($"{applicationName}/appsettings.json",
                        source =>
                        {
                            source.ReloadOnChange = true;
                            source.ConsulConfigurationOptions = ConsulConfig;
                        });
                    builder.AddConsul($"{applicationName}/appsettings.{environmentName}.json",
                        source =>
                        {
                            source.Optional = true;
                            source.ConsulConfigurationOptions = ConsulConfig;
                        });
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

    }
}
