using CityShobTaskListClient.TaskUpdatedServiceReference;
using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityShobTaskListClient.Model
{
    /// <summary>
    /// A handler for receiving notifications about user task updates from the server.
    /// </summary>
    public class TaskUpdatedServiceCallbackHandler: ITaskUpdatedServiceCallback
    {
        /// <summary>
        /// The logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Notifies when a user task is added.
        /// </summary>
        public event EventHandler<UserTaskEventArgs> UserTaskAdded;

        /// <summary>
        /// Notifies when a user task is updated.
        /// </summary>
        public event EventHandler<UserTaskEventArgs> UserTaskUpdated;

        /// <summary>
        /// Notifies when a user task is deleted.
        /// </summary>
        public event EventHandler<UserTaskEventArgs> UserTaskDeleted;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="logger"></param>
        public TaskUpdatedServiceCallbackHandler(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Notifies when a user task is added.
        /// </summary>
        /// <param name="userTask"></param>
        public void OnTaskAdded(Common.UserTask userTask)
        {
            UserTaskAdded?.Invoke(this, new UserTaskEventArgs { Id = userTask.Id, UserTask = userTask });
        }

        /// <summary>
        /// Notifies when a user task is updfated.
        /// </summary>
        public void OnTaskUpdated(Common.UserTask userTask)
        {
            UserTaskUpdated?.Invoke(this, new UserTaskEventArgs { Id = userTask.Id, UserTask = userTask });
        }

        /// <summary>
        /// Notifies when a user task is deleted.
        /// </summary>
        public void OnTaskDeleted(Guid taskId)
        {
            UserTaskDeleted?.Invoke(this, new UserTaskEventArgs { Id = taskId, UserTask = null });
        }
    }

    /// <summary>
    /// Wraps a client endpoint for getting notified of user task updates in the server.
    /// </summary>
    public class TaskUpdatedClient : ITaskUpdatedClient
    {
        // The client endpoint for the TaskUpdatedService
        private TaskUpdatedServiceClient _client;

        private bool disposedValue;

        /// <summary>
        /// The logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// The service callback handler for user task updates.
        /// </summary>
        public TaskUpdatedServiceCallbackHandler CallbackHandler { get; set; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TaskUpdatedClient(ILogger<TaskUpdatedClient> logger)
        {
            Logger = logger; 
        }

        /// <summary>
        /// Initialize the client.
        /// </summary>
        public void Initialize()
        {
            try
            {
                CallbackHandler = new TaskUpdatedServiceCallbackHandler(Logger);
                _client = new TaskUpdatedServiceClient(new System.ServiceModel.InstanceContext(CallbackHandler), "NetTcpTaskUpdatedService");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error connecting to TaskUpdatedServiceClient: {ex.Message}");
                throw new InvalidOperationException("Error connecting to TaskUpdatedServiceClient.", ex);
            }

            try
            {
                _client.Subscribe();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error subscribing to user task updates: {ex.Message}");
                throw;
            }

            Logger.LogInformation("TaskUpdatedServiceCallbackHandler initialized.");
        }

        /// <summary>
        /// Unregisters the client from the server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _client?.Unsubscribe();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error unsubscribing from user task updates: {ex.Message}");
                // Not throwing exception: we're already done here.
            }
        }
    }
}
