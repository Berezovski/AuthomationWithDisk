using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Authomation
{
    class Startup
    {
        public static IConfiguration BuildConfiguration()
        {
            var envName = Environment.GetEnvironmentVariable("ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Environment.CurrentDirectory, "JsonSettings"))
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{envName}.json", true)
                .AddJsonFile($"appsettings.{envName}.User.json", true);

            return configuration.Build();
        }

        public static ILogger CreateLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
