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
    /// Interface for a a service that manages modifyig tasks in a to-do list.
    /// </summary>
    [ServiceContract]
    public interface IModifyTaskService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// The repository that stores user tasks.
        /// </summary>
        IUserTaskRepository UserTaskRepository { get; set; }

        /// <summary>
        /// Asynchronously adds a new task.
        /// </summary>
        /// <param name="userTask"> The task to add.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [OperationContract]
        [FaultContract(typeof(ModifyTaskError))]
        Task<bool> AddTaskAsync(Common.UserTask userTask);

        /// <summary>
        /// Asynchronously update an existing task.
        /// </summary>
        /// <param name="userTask"> The task to update.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [OperationContract]
        [FaultContract(typeof(ModifyTaskError))]
        Task<bool> UpdateTaskAsync(Common.UserTask userTask);

        /// <summary>
        /// Asynchronously delete an existing task.
        /// </summary>
        /// <param name="taskId"> The id of the task to delete.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [OperationContract]
        [FaultContract(typeof(ModifyTaskError))]
        Task<bool> DeleteTaskAsync(Guid taskId);

        /// <summary>
        /// Asynchronously gets all existing user tasks.
        /// </summary>
        /// <returns>All existing user tasks.</returns>
        [OperationContract]
        [FaultContract(typeof(ModifyTaskError))]
        Task<IEnumerable<Common.UserTask>> GetAllTasksAsync();
    }
}
