using CityShobTaskListServer.Repositories;
using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CityShobTaskListServer.Services
{
    /// <summary>
    /// Service that publishes changes to tasks in a to-do list.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class TaskUpdatedService : ITaskUpdatedService
    {
        // Subscribed clients
        private readonly ConcurrentDictionary<ITaskUpdatedCallback, object> _clients = new ConcurrentDictionary<ITaskUpdatedCallback, object>();

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
        public TaskUpdatedService(IUserTaskRepository userTaskRepository, ILogger<TaskUpdatedService> logger)
        {
            UserTaskRepository = userTaskRepository;
            Logger = logger;

            UserTaskRepository.UserTaskAdded += NotifyTaskAdded;
            UserTaskRepository.UserTaskUpdated += NotifyTaskUpdated;
            UserTaskRepository.UserTaskDeleted += NotifyTaskDeleted;

            Logger.LogInformation("TaskUpdatedService initialized.");
        }

        /// <summary>
        /// Subscribe to task updates.
        /// </summary>
        public void Subscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<ITaskUpdatedCallback>();
            _clients.TryAdd(callback, null);
        }

        /// <summary>
        /// Unsubscribe from task updates.
        /// </summary>
        public void Unsubscribe()
        {
            var callback = OperationContext.Current.GetCallbackChannel<ITaskUpdatedCallback>();
            _clients.TryRemove(callback, out _);
        }

        //TODO: Consider using Parallel.ForEach to improve performance

        public void NotifyTaskAdded(object sender, UserTaskEventArgs e)
        {
            Parallel.ForEach(_clients.Keys, client =>
            {
                try
                {
                    client.OnTaskAdded(e.UserTask);
                }
                catch
                {
                    Logger.LogError($"Failed to notify client about new user task: {e.UserTask.Id}");
                    //throw new FaultException("An error occurred while notifying the client about a new user task.");
                }
            });
        }

        public void NotifyTaskUpdated(object sender, UserTaskEventArgs e)
        {
            Parallel.ForEach(_clients.Keys, client =>
            {
                try
                {
                    client.OnTaskUpdated(e.UserTask);
                }
                catch
                {
                    Logger.LogError($"Failed to notify client about updated user task: {e.UserTask.Id}");
                    //throw new FaultException("An error occurred while notifying the client about an updated user task.");
                }
            });
        }

        public void NotifyTaskDeleted(object sender, UserTaskEventArgs e)
        {
            Parallel.ForEach(_clients.Keys, client =>
            {
                try
                {
                    client.OnTaskDeleted(e.Id);
                }
                catch
                {
                    Logger.LogError($"Failed to notify client about deleted user task: {e.UserTask.Id}");
                    //throw new FaultException("An error occurred while notifying the client about a deleted user task.");
                }
            });
        }
    }
}