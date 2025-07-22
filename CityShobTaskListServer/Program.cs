using CityShobTaskListServer.Repositories;
using CityShobTaskListServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CityShobTaskListServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Use dependency injection to initialize the server components
            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var server = serviceProvider.GetService<Application>();

            // Run the server logic
            server.Run();
        }

        /// <summary>
        /// Register the services and dependencies for the server application.
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureServices(IServiceCollection services)
        {
            // Register logger
            services.AddLogging(builder => builder.AddConsole());

            // Register the database context
            //services.AddSingleton<UserTasksDbContext>();

            // Register repositories
            services.AddSingleton<IUserTaskRepository, UserTaskRepository>();

            // Register services
            services.AddSingleton<IModifyTaskService, ModifyTaskService>();
            services.AddSingleton<ITaskUpdatedService, TaskUpdatedService>();

            // Register the server entry point
            services.AddSingleton<Application>();
        }

        internal class Application
        {
            ILogger _logger;
            IUserTaskRepository _userTaskRepository;
            IModifyTaskService _modifyTaskService;
            ITaskUpdatedService _taskUpdatedService;

            public Application(ILogger<Application> logger,
                IUserTaskRepository userTaskRepository,
                IModifyTaskService modifyTaskService,
                ITaskUpdatedService taskUpdatedService)
            {
                _logger = logger;
                _userTaskRepository = userTaskRepository;
                _modifyTaskService = modifyTaskService;
                _taskUpdatedService = taskUpdatedService;
            }

            public void Run()
            {
                var modifyTaskServiceAddress = new Uri("net.tcp://localhost:8080/ModifyTaskService");
                var taskUpdatedServiceAddress = new Uri("net.tcp://localhost:8080/TaskUpdatedService");

                var serviceBehavior = new ServiceMetadataBehavior
                {
                    MetadataExporter = { PolicyVersion = PolicyVersion.Policy15 }
                };

                // Create and run the services
                var modifyTaskServiceHost = new ServiceHost(_modifyTaskService, modifyTaskServiceAddress);
                modifyTaskServiceHost.Description.Behaviors.Add(serviceBehavior);   
                modifyTaskServiceHost.AddServiceEndpoint(typeof(IModifyTaskService), new NetTcpBinding(SecurityMode.None), "");
                modifyTaskServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
                modifyTaskServiceHost.Open();

                var taskUpdatedServiceHost = new ServiceHost(_taskUpdatedService, taskUpdatedServiceAddress);
                taskUpdatedServiceHost.Description.Behaviors.Add(serviceBehavior);
                taskUpdatedServiceHost.AddServiceEndpoint(typeof(ITaskUpdatedService), new NetTcpBinding(SecurityMode.None), "");
                taskUpdatedServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
                taskUpdatedServiceHost.Open();

                _logger.LogInformation("CityShob Task List Server started.");

                Console.WriteLine("CityShob Task List Server running...");
                Console.WriteLine("Press any key to stop the server.");

                while (true)
                {
                    // Keep the server running
                    if (Console.KeyAvailable)
                    {
                        break;
                    }
                }
            }
        }
    }
}
