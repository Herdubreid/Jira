using BlazorState;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Config file
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile($"{appData}/Microsoft/UserSecrets/723ebf55-e42b-4b0e-99c8-91277eb6e66f/secrets.json", false, true)
                    .Build();

                // Initialise the Logger
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder
                        .AddConfiguration(config.GetSection("Logging"))
                        .AddConsole();
                });
                ILogger logger = loggerFactory.CreateLogger<Program>();

                // Build the Service Collection
                var services = new ServiceCollection()
                    .AddSingleton(logger)
                    .AddSingleton(config)
                    .AddSingleton<Celin.AIS.Server, E1>()
                    .AddSingleton<Celin.Jira.Server, Jira>()
                    .AddBlazorState(options =>
                    {
                        options.UseCloneStateBehavior = false;
                        options.Assemblies = new Assembly[]
                        {
                            typeof(Celin.OMWPlannerState).GetTypeInfo().Assembly
                        };
                    })
                    .AddSingleton<Celin.OMWPlannerState>()
                    .BuildServiceProvider();

                // Build the Command Line Parser
                var app = new CommandLineApplication<Cmd>();
                app.Conventions
                    .UseDefaultConventions()
                    .UseConstructorInjection(services);
                app.OnExecute(() => app.ShowHelp());

                app.Execute(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
        }
    }
}
