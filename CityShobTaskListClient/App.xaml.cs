using CityShobTaskListClient.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CityShobTaskListClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly AppBootstrapper _bootstrapper;

        public App()
        {
            _bootstrapper = new AppBootstrapper();

            InitializeComponent();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ITaskUpdatedClient taskUpdatedClient = (ITaskUpdatedClient)_bootstrapper.Services.GetService(typeof(ITaskUpdatedClient));
            taskUpdatedClient.Disconnect();
        }
    }
}
