using Common;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CityShobTaskListClient.Model
{
    /// <summary>
    /// Interface for a repository that stores user tasks.
    /// </summary>
    public interface IUserTaskRepository
    {
        /// <summary>
        /// The logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// The service endpoint for modifying user tasks.
        /// </summary>
        IModifyTaskClient ModifyTaskClient { get; set; }

        /// <summary>
        /// The service endpoint for user task changes notifications.
        /// </summary>
        ITaskUpdatedClient TaskUpdatedClient { get; set; }

        /// <summary>
        /// The user tasks model.
        /// </summary>
        ObservableCollection<UserTask> UserTasks { get; set; }

        /// <summary>
        /// Initialize the user tasks repository.
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();

        /// <summary>
        /// Add a new user task to the repository.
        /// </summary>
        /// <param name="userTask">The user task to add.</param>
        /// <returns></returns>
        Task AddUserTaskAsync(UserTask userTask);

        /// <summary>
        /// Updates a user task in the repository.
        /// </summary>
        /// <param name="userTask">The user task to update.</param>
        /// <returns></returns>
        Task UpdateUserTaskAsync(UserTask userTask);

        /// <summary>
        /// Deletes a user task from the repository.
        /// </summary>
        /// <param name="userTask">The user task to delete.</param>
        /// <returns></returns>
        Task DeleteUserTaskAsync(UserTask userTask);
    }
}