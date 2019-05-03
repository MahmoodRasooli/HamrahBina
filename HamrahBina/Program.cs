using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HamrahBina
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.CreateText("logfile.txt"))
                {
                    writer.Write("start");
                }

                CreateWebHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                using (StreamWriter writer = System.IO.File.CreateText("logfile.txt"))
                {
                    writer.Write(ex.ToString());
                }
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
