using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Refit;
using FunctionAppImagensNASA.HttpClients;
using FunctionAppImagensNASA.Data;

namespace FunctionAppImagensNASA
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services => {
                    services.AddRefitClient<IImagemDiariaAPI>()
                        .ConfigureHttpClient(
                            c => c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EndpointNASA")));

                    services.AddDbContext<NASAContext>(
                        options => options.UseSqlServer(
                            Environment.GetEnvironmentVariable("BaseImagensNASA")));
                })
                .Build();

            host.Run();
        }
    }
}