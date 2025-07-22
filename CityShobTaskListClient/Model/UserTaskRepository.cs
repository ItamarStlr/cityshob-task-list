using Caliburn.Micro;
using CityShobTaskListClient.TaskUpdatedServiceReference;
using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace CityShobTaskListClient.Model
{
    /// <summary>
    /// Stores user tasks.
    /// </summary>
    public class UserTaskRepository : IUserTaskRepository
    {
        // Lock object
        private object _lock = new object();

        // The UI synchronization context to marshal updates to the UI thread
        private SynchronizationContext _uiContext;

        /// <summary>
        /// The logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// The service endpoint for modifying user tasks.
        /// </summary>
        public IModifyTaskClient ModifyTaskClient { get; set; }

        /// <summary>
        /// The service endpoint for user task changes notifications.
        /// </summary>
        public ITaskUpdatedClient TaskUpdatedClient { get; set; }

        /// <summary>
        /// The user tasks model.
        /// </summary>
        public ObservableCollection<UserTask> UserTasks { get; set; } = new ObservableCollection<UserTask>();

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="modifyTaskClient">The service endpoint for modifying user tasks.</param>
        /// <param name="taskUpdatedClient">The service endpoint for user task changes notifications.</param>
        /// <param name="logger">The logger.</param>
        public UserTaskRepository(IModifyTaskClient modifyTaskClient, ITaskUpdatedClient taskUpdatedClient, ILogger<UserTaskRepository> logger)
        {
            _uiContext = SynchronizationContext.Current;
            Logger = logger;

            ModifyTaskClient = modifyTaskClient;
            TaskUpdatedClient = taskUpdatedClient;
        }

        /// <summary>
        /// Initialize the user tasks repository.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            try
            {
                ModifyTaskClient.Initialize();
                TaskUpdatedClient.Initialize();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to initialize client services: {ex.Message}");
                throw;
            }

            try
            {
                // Get all tasks from the server
                var result = await ModifyTaskClient.GetAllTasksAsync();
                foreach (var task in result)
                {
                    // Ensure the task is not already in the collection
                    if (!UserTasks.Any(t => t.Id == task.Id))
                    {
                        UserTasks.Add(task);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Failed to initialize user tasks repository: {ex.Message}");
                throw;
            }

            // Subscribe to user task update events
            TaskUpdatedClient.CallbackHandler.UserTaskAdded += OnUserTaskAddedOnServer;
            TaskUpdatedClient.CallbackHandler.UserTaskUpdated += OnUserTaskUpdatedOnServer;
            TaskUpdatedClient.CallbackHandler.UserTaskDeleted += OnUserTaskDeletedOnServer;
        }

        /// <summary>
        /// Add a new user task to the repository.
        /// </summary>
        /// <param name="userTask">The user task to add.</param>
        /// <returns></returns>
        public async Task AddUserTaskAsync(UserTask userTask)
        {
            try
            {
                // Add the task to the server
                bool result = await ModifyTaskClient.AddTaskAsync(userTask);
                if (!result)
                {
                    Logger.LogError($"Failed to add user task: {userTask.Description}");
                    throw new InvalidOperationException($"Failed to add user task: {userTask.Description}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding user task: {ex.Message}");
                throw;
            }

            // The task will be added to the UserTasks collection via the event handler OnUserTaskAddedOnServer.
        }

        /// <summary>
        /// Updates a user task in the repository.
        /// </summary>
        /// <param name="userTask">The user task to update.</param>
        /// <returns></returns>
        public async Task UpdateUserTaskAsync(UserTask userTask)
        {
            try
            {
                // Update the task on the server
                bool result = await ModifyTaskClient.UpdateTaskAsync(userTask);
                if (!result)
                {
                    Logger.LogError($"Failed to update user task: {userTask.Description}");
                    throw new InvalidOperationException($"Failed to update user task: {userTask.Description}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating user task: {ex.Message}");
                throw;
            }

            // The task will be updated in the UserTasks collection via the event handler OnUserTaskUpdatedOnServer.
        }

        /// <summary>
        /// Deletes a user task from the repository.
        /// </summary>
        /// <param name="userTask">The user task to delete.</param>
        /// <returns></returns>
        public async Task DeleteUserTaskAsync(UserTask userTask)
        {
            try
            {
                // Delete the task on the server
                bool result = await ModifyTaskClient.DeleteTaskAsync(userTask.Id);
                if (!result)
                {
                    Logger.LogError($"Failed to delete user task: {userTask.Description}");
                    throw new InvalidOperationException($"Failed to delete user task: {userTask.Description}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error deleting user task: {ex.Message}");
                throw;
            }

            // The task will be removed from the UserTasks collection via the event handler OnUserTaskDeletedOnServer.
        }

        /// <summary>
        /// Handle user task additions from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserTaskAddedOnServer(object sender, UserTaskEventArgs e)
        {
            // Marshal back to the UI thread before touching UserTasks
            _uiContext.Post(_ =>
            {
                lock (_lock)
                {
                    // Check if the task already exists
                    if (UserTasks.Any(t => t.Id == e.UserTask.Id))
                        return;

                    UserTasks.Add(e.UserTask);
                }
            }, null);
        }

        /// <summary>
        /// Handle user task updates from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserTaskUpdatedOnServer(object sender, UserTaskEventArgs e)
        {
            lock (_lock)
            {
                var task = UserTasks.FirstOrDefault(t => t.Id == e.UserTask.Id);
                if (task != null)
                {
                    // Update existing task in-place
                    task.Description = e.UserTask.Description;
                    task.IsLocked = e.UserTask.IsLocked;
                    task.IsCompleted = e.UserTask.IsCompleted;
                }
            }
        }

        /// <summary>
        /// Handle user task deletions from the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserTaskDeletedOnServer(object sender, UserTaskEventArgs e)
        {
            lock (_lock)
            {
                int index = UserTasks.IndexOf(UserTasks.FirstOrDefault(t => t.Id == e.Id));
                if (index < 0)
                {
                    Logger.LogInformation($"User task with ID {e.UserTask.Id} not found for update.");
                    return;
                }

                UserTasks.RemoveAt(index);
            }
        }
    }
}
