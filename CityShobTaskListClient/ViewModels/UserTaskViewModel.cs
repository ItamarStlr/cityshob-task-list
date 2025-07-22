using Caliburn.Micro;
using CityShobTaskListClient.Model;
using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CityShobTaskListClient.ViewModels
{
    public class UserTaskViewModel : Screen
    {
        /// <summary>
        /// The logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// The user tasks repository.
        /// </summary>
        public IUserTaskRepository UserTaskRepository { get; set; }

        /// <summary>
        /// The user tasks model
        /// </summary>
        public ObservableCollection<UserTask> UserTasks => UserTaskRepository.UserTasks;

        private UserTask _selectedUserTask;  // The currently selected user task.

        private string _originalDescription;  // The original task description before editing.

        private bool _isEditingDescription;  // Flag to indicate if the task description is being edited.

        private bool _isSavingDescription;  // Flag to indicate if the description is being saved.

        /// <summary>
        /// The currently selected user task.
        /// </summary>
        public UserTask SelectedUserTask
        {
            get
            {
                return _selectedUserTask;
            }
            set
            {
                _selectedUserTask = value;
                NotifyOfPropertyChange(() => SelectedUserTask);
                NotifyOfPropertyChange(() => CanEditUserTask);
                NotifyOfPropertyChange(() => CanDeleteUserTask);
            }
        }

        /// <summary>
        /// Inicates if the currently selected task is being edited.
        /// </summary>
        public bool IsEditingDescription
        {
            get => _isEditingDescription;
            private set
            {
                _isEditingDescription = value;
                NotifyOfPropertyChange(() => IsEditingDescription);
            }
        }

        /// <summary>
        /// Can the current selected user task be edited.
        /// </summary>
        public bool CanEditUserTask => SelectedUserTask != null && SelectedUserTask.IsLocked == false;

        /// <summary>
        /// Can the current selected user task be deleted.
        /// </summary>
        public bool CanDeleteUserTask => SelectedUserTask != null && SelectedUserTask.IsLocked == false;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="userTaskRepository">The user tasks repository.</param>
        /// <param name="logger">The logger.</param>
        public UserTaskViewModel(IUserTaskRepository userTaskRepository, ILogger<UserTaskViewModel> logger)
        {
            UserTaskRepository = userTaskRepository;
            Logger = logger;

            UserTasks.CollectionChanged += HandleCollectionChanged;
        }

        /// <summary>
        /// Handle changes to the user events collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedUserTask == null && UserTasks.Any())
            {
                SelectedUserTask = UserTasks.First();
            }
        }

        /// <summary>
        /// Initializes the ViewModel.
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            await base.OnInitializeAsync(cancellationToken);
            try
            {
                await UserTaskRepository.InitializeAsync();
            }
            catch (Exception ex)
            {
                HandleError("Error initializing", ex);
            }
        }

        /// <summary>
        /// Adds a new user task.
        /// </summary>
        /// <returns></returns>
        public async Task AddUserTask()
        {
            var newTask = new UserTask("New Task");

            try
            {
                await UserTaskRepository.AddUserTaskAsync(newTask);
            }
            catch (Exception ex)
            {
                HandleError("Error adding new task", ex);
            }
        }

        /// <summary>
        /// Edits the selected user task
        /// </summary>
        /// <returns></returns>
        public async Task EditUserTask()
        {
            if (SelectedUserTask != null)
            {
                try
                {
                    SelectedUserTask.IsLocked = false; 
                    await UserTaskRepository.UpdateUserTaskAsync(SelectedUserTask);
                }
                catch (Exception ex)
                {
                    HandleError("Error editing task", ex);
                }
            }
        }

        /// <summary>
        /// Edit the complete state of the selected user task.
        /// </summary>
        /// <returns></returns>
        public async Task ToggleCompleteUserTask()
        {
            if (SelectedUserTask != null)
            {
                try
                {
                    await UserTaskRepository.UpdateUserTaskAsync(SelectedUserTask);
                }
                catch (Exception ex)
                {
                    HandleError("Error editing task", ex);
                }
            }
        }

        /// <summary>
        /// Deletes the currently selected user task.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteUserTask()
        {
            if (SelectedUserTask != null)
            {
                try
                {
                    await UserTaskRepository.DeleteUserTaskAsync(SelectedUserTask);
                }
                catch (Exception ex)
                {
                    HandleError("Error deleting task", ex);
                }
            }
        }

        /// <summary>
        /// Locks the currently selected user task for editing.
        /// </summary>
        /// <returns></returns>
        public async Task LockUserTask()
        {
            if (SelectedUserTask != null)
            {
                try
                {
                    SelectedUserTask.IsLocked = true;
                    await UserTaskRepository.UpdateUserTaskAsync(SelectedUserTask);
                }
                catch (Exception ex)
                {
                    HandleError("Error locking task", ex);
                }
            }
        }

        /// <summary>
        /// Unlocks the currently selected user task for editing.
        /// </summary>
        /// <returns></returns>
        public async Task UnlockUserTask()
        {
            if (SelectedUserTask != null)
            {
                try
                {
                    SelectedUserTask.IsLocked = false;
                    await UserTaskRepository.UpdateUserTaskAsync(SelectedUserTask);
                }
                catch (Exception ex)
                {
                    HandleError("Error unlocking task", ex);
                }
            }
        }

        public async Task BeginEditDescription()
        {
            if (SelectedUserTask == null || SelectedUserTask.IsLocked) return;

            _originalDescription = SelectedUserTask.Description;
            IsEditingDescription = true;
            await LockUserTask();
        }

        /// <summary>
        /// Cancel the edit of the user description and return the original description.
        /// </summary>
        /// <returns></returns>
        public async Task CancelEditDescription()
        {
            if (_isSavingDescription)
            {
                return;
            }

            if (IsEditingDescription && SelectedUserTask != null)
            {
                SelectedUserTask.Description = _originalDescription;
                _originalDescription = null;
                IsEditingDescription = false;
                await UnlockUserTask();
            }
        }

        /// <summary>
        /// Save the edited user description.
        /// </summary>
        /// <returns></returns>
        public async Task SaveEditDescription()
        {
            if (_isEditingDescription && SelectedUserTask != null)
            {
                _originalDescription = null;
                IsEditingDescription = false;
                await EditUserTask();
                _isSavingDescription = false; 
            }
        }

        /// <summary>
        /// Indicate that the description is about to be saved
        /// </summary>
        /// <remarks> Aimed to prevent lost focus when clicking the save button from cancelling the edit.</remarks>
        public void BeginSaveDescription()
        {
            _isSavingDescription = true;
        }

        /// <summary>
        /// Display errors to the user
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The exception.</param>
        private void HandleError(string message, Exception ex)
        {
            // Log the full error for debugging purposes
            Logger.LogError(ex, message);

            // Show the error to the user via a message box
            System.Windows.MessageBox.Show(
                $"{message}\n\n{ex.Message}",
                "Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }
}
