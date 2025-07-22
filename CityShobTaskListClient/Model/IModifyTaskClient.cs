using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CityShobTaskListClient.Model
{
    /// <summary>
    /// Interface for a client endpoint for modifying user tasks in the server.
    /// </summary>
    public interface IModifyTaskClient
    {
        /// <summary>
        /// The logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Initialize the client.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Add a new task to the server.
        /// </summary>
        /// <param name="userTask">The user task to add.</param>
        /// <returns></returns>
        Task<bool> AddTaskAsync(UserTask userTask);

        /// <summary>
        /// Modifies an existing task on the server.
        /// </summary>
        /// <param name="userTask">The user task to modify.</param>
        /// <returns></returns>
        Task<bool> UpdateTaskAsync(UserTask userTask);

        /// <summary>
        /// Deletes a task from the server by its ID.
        /// </summary>
        /// <param name="taskId">The ID of the user task to delete.</param>
        /// <returns></returns>
        Task<bool> DeleteTaskAsync(Guid taskId);

        /// <summary>
        /// Get all existing tasks from the server.
        /// </summary>
        /// <returns>All existing user tasks.</returns>
        Task<UserTask[]> GetAllTasksAsync();
    }
}