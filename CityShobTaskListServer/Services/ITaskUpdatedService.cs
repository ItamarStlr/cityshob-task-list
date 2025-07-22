using CityShobTaskListServer.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.ServiceModel;

namespace CityShobTaskListServer.Services
{
    /// <summary>
    /// Interface for a service that publishes changes to tasks.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(ITaskUpdatedCallback))]
    public interface ITaskUpdatedService
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
        /// Subscribe to task updates.
        /// </summary>
        [OperationContract]
        void Subscribe();

        /// <summary>
        /// Unsubscribe from task updates.
        /// </summary>
        [OperationContract]
        void Unsubscribe();
    }
}