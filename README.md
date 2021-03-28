# How to use consul with dotnet 5 for configuration management

##### 1. Starting consul with docker

docker run
-d
-p 8500:8500
-p 8600:8600/udp
--name=badger
consul agent -server -ui -node=server-1 -bootstrap-expect=1 -client=0.0.0.0

Open `http://localhost:8500/ui/dc1/kv` and create new key

Name = `ApplicationName`/appsettings.json

##### 2. Create .net 5 app

```
dotnet new api --name NotificationAPI --no-https
```

##### 3.Program.cs configuration for consul

```
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
```

##### 4. Load conful file

```
{
  "Message": "Hello World from Development",
  "PushNotificationOptions":{
   "ApiHost":"http://api.sms2.com/"
  }
}
```

##### 5. Get Configuration Data

`Configuration["Message"]`

##### 6. Use IOptionsSnapshot

If you want the values of your class created according to the options pattern to change when your data changes, you should use IOptionsSnapshot. (Ref = `` PushNotificationsController``)


##### Ref

https://medium.com/@peacecwz/net-core-consul-configuration-kullan%C4%B1m%C4%B1-16e3e068b18c

https://www.c-sharpcorner.com/article/dynamic-asp-net-core-configurations-with-consul-kv/

https://www.natmarchand.fr/consul-configuration-aspnet-core/
