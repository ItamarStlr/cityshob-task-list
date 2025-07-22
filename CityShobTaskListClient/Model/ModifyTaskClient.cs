using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.ServiceModel;
using CityShobTaskListClient.ModifyTaskServiceReference;
using Microsoft.Extensions.Logging;

namespace CityShobTaskListClient.Model
{
    /// <summary>
    /// Wraps a client endpoint for modifying user tasks in the server.
    /// </summary>
    public class ModifyTaskClient : IModifyTaskClient
    {
        // The client endpoint for the ModifyTaskService
        private ModifyTaskServiceClient _client;

        /// <summary>
        /// The logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="logger"></param>
        public ModifyTaskClient(ILogger<ModifyTaskClient> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Initialize the client.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Initialize()
        {
            try
            {
                _client = new ModifyTaskServiceClient("NetTcpModifyTaskService");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error connecting to ModifyTaskServiceClient: {ex.Message}");
                throw new InvalidOperationException("Error connecting to ModifyTaskServiceClient.", ex);
            }

            Logger.LogInformation("ModifyTaskClient initialized.");
        }

        /// <summary>
        /// Add a new task to the server.
        /// </summary>
        /// <param name="userTask">The user task to add.</param>
        /// <returns></returns>
        public async Task<bool> AddTaskAsync(Common.UserTask userTask)
        {
            try
            {
                return await _client.AddTaskAsync(userTask);
            }
            catch (FaultException<ModifyTaskError> fault)
            {
                // Handle specific fault exceptions
                Logger.LogError($"Error adding user task: {fault.Detail.Message}");
                throw fault;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Logger.LogError($"Error adding user task: {ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// Modifies an existing task on the server.
        /// </summary>
        /// <param name="userTask">The user task to modify.</param>
        /// <returns></returns>
        public async Task<bool> UpdateTaskAsync(Common.UserTask userTask)
        {
            try
            {
                return await _client.UpdateTaskAsync(userTask);
            }
            catch (FaultException<ModifyTaskError> fault)
            {
                // Handle specific fault exceptions
                Logger.LogError($"Error modifying user task: {fault.Detail.Message}");
                throw fault;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Logger.LogError($"Error modifying user task: {ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// Deletes a task from the server by its ID.
        /// </summary>
        /// <param name="taskId">The ID of the user task to delete.</param>
        /// <returns></returns>
        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {
            try
            {
                return await _client.DeleteTaskAsync(taskId);
            }
            catch (FaultException<ModifyTaskError> fault)
            {
                // Handle specific fault exceptions
                Logger.LogError($"Error deleting user task: {fault.Detail.Message}");
                throw fault;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Logger.LogError($"Error deleting user task: {ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// Get all existing tasks from the server.
        /// </summary>
        /// <returns>All existing user tasks.</returns>
        public async Task<Common.UserTask[]> GetAllTasksAsync()
        {
            try
            {
                return await _client.GetAllTasksAsync();
            }
            catch (FaultException<ModifyTaskError> fault)
            {
                // Handle specific fault exceptions
                Logger.LogError($"Error retrieving all user tasks: {fault.Detail.Message}");
                throw fault;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                Logger.LogError($"Error retrieving all user tasks: {ex.Message}");
                throw ex;
            }
        }
    }
}
