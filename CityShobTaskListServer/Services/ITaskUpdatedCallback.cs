using System;
using System.ServiceModel;
using Common;

namespace CityShobTaskListServer.Services
{
    /// <summary>
    /// Interface for a callback service that publishes changes to tasks.
    /// </summary>
    public interface ITaskUpdatedCallback
    {
        /// <summary>
        /// Notifies that a new task was added.
        /// </summary>
        /// <param name="task">The new task.</param>
        [OperationContract(IsOneWay = true)]
        void OnTaskAdded(Common.UserTask task);

        /// <summary>
        /// Notifies that an existing task was updated.
        /// </summary>
        /// <param name="task">The updated task.</param>
        [OperationContract(IsOneWay = true)]
        void OnTaskUpdated(Common.UserTask task);

        /// <summary>
        /// Notifies that an existing task was deleted.
        /// </summary>
        /// <param name="taskId">The deleted task's ID.</param>
        [OperationContract(IsOneWay = true)]
        void OnTaskDeleted(Guid taskId);
    }
}