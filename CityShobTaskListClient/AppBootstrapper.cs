using Caliburn.Micro;
using CityShobTaskListClient.Model;
using CityShobTaskListClient.Views;
using CityShobTaskListClient.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CityShobTaskListClient
{
    /// <summary>
    /// Bootstraps the client application and wires the dependencies. 
    /// </summary>
    /// <remarks> Relies on the Caliburn bootstrapper and MS dependency injection.</remarks>
    public class AppBootstrapper : BootstrapperBase
    {
        public IServiceProvider Services { get; set; }

        /// <summary>
        /// Initialize the bootstrapper
        /// </summary>
        public AppBootstrapper()
        {
            Initialize();
        }

        /// <summary>
        /// Register the services and dependencies for the client application.
        /// </summary>
        protected override void Configure()
        {
            var services = new ServiceCollection();

            // Register Caliburn core services
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<IEventAggregator, EventAggregator>();

            // Register logger
            services.AddLogging(builder => builder.AddConsole());

            // Register Model and client services
            services.AddSingleton<IModifyTaskClient, ModifyTaskClient>();
            services.AddSingleton<ITaskUpdatedClient, TaskUpdatedClient>();
            services.AddSingleton<IUserTaskRepository, UserTaskRepository>();

            // Registes ViewModels
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<UserTaskViewModel>();

            // Registes Views
            services.AddTransient<MainWindowView>();
            services.AddTransient<UserTaskView>();

            Services = services.BuildServiceProvider();
        }

        // Resolve a single instance (Caliburn)
        protected override object GetInstance(Type service, string key)
        {
            if (!string.IsNullOrEmpty(key))
                return Services.GetRequiredService(Type.GetType(key));

            return Services.GetRequiredService(service);
        }

        // Resolve IEnumerable<T> (Caliburn)
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Services.GetServices(service);
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<MainWindowViewModel>();
        }
    }
}
