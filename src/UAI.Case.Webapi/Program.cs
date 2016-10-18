using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace UAI.Case.Webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var config = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                    //.AddJsonFile("hosting.json", optional: true)
                    .AddCommandLine(args)
                  .Build(); 

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseConfiguration(config)
                .Build();
                

            host.Run();
        }

    }
}
