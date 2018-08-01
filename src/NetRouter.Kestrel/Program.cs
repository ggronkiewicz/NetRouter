namespace NetRouter.Kestrel
{
    using System.Net;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(options => options.Listen(IPAddress.Any, 8000))
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .ConfigureAppConfiguration(builder =>
                {
                    builder
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddEnvironmentVariables();
                } )
                .Build();

            host.Run();
        }
    }
}
