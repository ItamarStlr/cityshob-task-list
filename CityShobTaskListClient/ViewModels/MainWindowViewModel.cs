using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityShobTaskListClient.ViewModels
{
    public class MainWindowViewModel : Conductor<Screen>.Collection.OneActive
    {
        /// <summary>
        /// The user tasks view model.
        /// </summary>
        public UserTaskViewModel UserTasksVm { get; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="userTasksVm">The user tasks view model.</param>
        public MainWindowViewModel(UserTaskViewModel userTasksVm)
        {
            UserTasksVm = userTasksVm;
        }

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            await base.OnInitializeAsync(cancellationToken);
            await ActivateItemAsync(UserTasksVm, cancellationToken);
        }
    }
}
