using CityShobTaskListServer.Repositories;
using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CityShobTaskListServer.Services
{
    /// <summary>
    /// Service that manages modifying tasks in a to-do list.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ModifyTaskService : IModifyTaskService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// The repository that stores user tasks.
        /// </summary>
        public IUserTaskRepository UserTaskRepository { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userTaskRepository">The user task repository.</param>
        /// <param name="logger">The logger.</param>
        public ModifyTaskService(IUserTaskRepository userTaskRepository, ILogger<ModifyTaskService> logger)
        {
            UserTaskRepository = userTaskRepository;
            Logger = logger;

            Logger.LogInformation("ModifyTaskService initialized.");
        }

        /// <summary>
        /// Asynchronously adds a new task.
        /// </summary>
        /// <param name="userTask"> The task to add.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<bool> AddTaskAsync(Common.UserTask userTask)
        {
            try
            {
                return await UserTaskRepository.AddTaskAsync(userTask);
            }
            catch (Exception ex)
            {
                throw new FaultException<ModifyTaskError>(
                    new ModifyTaskError { Message = ex.Message, Exception = ex },
                    new FaultReason("Error while adding new user task.")
                );
            }
        }

        /// <summary>
        /// Asynchronously delete an existing task.
        /// </summary>
        /// <param name="taskId"> The id of the task to delete.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {
            try
            {
                return await UserTaskRepository.DeleteTaskAsync(taskId);
            }
            catch (Exception ex)
            {
                throw new FaultException<ModifyTaskError>(
                    new ModifyTaskError { Message = ex.Message, Exception = ex },
                    new FaultReason("Error while deleting user task.")
                );
            }
        }

        /// <summary>
        /// Asynchronously update an existing task.
        /// </summary>
        /// <param name="userTask"> The task to update.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<bool> UpdateTaskAsync(Common.UserTask userTask)
        {
            try
            {
                return await UserTaskRepository.UpdateTaskAsync(userTask);
            }
            catch (Exception ex)
            {
                throw new FaultException<ModifyTaskError>(
                    new ModifyTaskError { Message = ex.Message, Exception = ex },
                    new FaultReason("Error while updating user task.")
                );
            }
        }

        /// <summary>
        /// Asynchronously gets all existing user tasks.
        /// </summary>
        /// <returns>All existing user tasks.</returns>
        public async Task<IEnumerable<Common.UserTask>> GetAllTasksAsync()
        {
            try
            {
                return await UserTaskRepository.GetAllTasksAsync();
            }
            catch (Exception ex)
            {
                throw new FaultException<ModifyTaskError>(
                    new ModifyTaskError { Message = ex.Message, Exception = ex },
                    new FaultReason("Error while retrieving existing user tasks.")
                );
            }
        }
    }
}
