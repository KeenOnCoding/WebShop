using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;

namespace WebShop
{
    public class Program
    {
        public static string Namespace = typeof(Startup).Namespace;
        public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            var host = BuildWebHost(configuration, args);

            host.Run();
        }
        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();


            return builder.Build();
        }
        public static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)

      .UseStartup<Startup>()
      .UseContentRoot(Directory.GetCurrentDirectory())
      .UseWebRoot("Pics")
      .Build();
    }
}
