using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityShobTaskListServer.Repositories
{
    public interface IUserTaskRepository
    {
        /// <summary>
        /// The logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Notifies when a user task is added.
        /// </summary>
        event EventHandler<UserTaskEventArgs> UserTaskAdded;

        /// <summary>
        /// Notifies when a user task is updated.
        /// </summary>
        event EventHandler<UserTaskEventArgs> UserTaskUpdated;

        /// <summary>
        /// Notifies when a user task is deleted.
        /// </summary>
        event EventHandler<UserTaskEventArgs> UserTaskDeleted;

        /// <summary>
        /// Adds a new user task to the repository.
        /// </summary>
        /// <param name="userTask">The task to add.</param>
        /// <returns></returns>
        Task<bool> AddTaskAsync(Common.UserTask userTask);

        /// <summary>
        /// Updates an existing user task in the repository.
        /// </summary>
        /// <param name="userTask">The task to update.</param>
        /// <returns></returns>
        Task<bool> UpdateTaskAsync(Common.UserTask userTask);

        /// <summary>
        /// Deletes an existing user task from the repository.
        /// </summary>
        /// <param name="taskId">The task to delete.</param>
        /// <returns></returns>
        Task<bool> DeleteTaskAsync(Guid taskId);

        /// <summary>
        /// Gets a user task by its ID.
        /// </summary>
        /// <param name="taskId">The task ID.</param>
        /// <returns>The requested task or null if none exists.</returns>
        Task<Common.UserTask> GetTaskAsync(Guid taskId);

        /// <summary>
        /// Gets all user tasks from the repository.
        /// </summary>
        /// <returns>All user tasks in the repository.</returns>
        Task<IEnumerable<Common.UserTask>> GetAllTasksAsync();
    }
}
