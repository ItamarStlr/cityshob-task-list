using CityShobTaskListClient.TaskUpdatedServiceReference;
using Microsoft.Extensions.Logging;

namespace CityShobTaskListClient.Model
{
    /// <summary>
    /// Interface for a client endpoint for getting notified of user task updates in the server.
    /// </summary>
    public interface ITaskUpdatedClient
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
        /// Unregisters the client from the server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// The service callback handler for user task updates.
        /// </summary>
        TaskUpdatedServiceCallbackHandler CallbackHandler { get; set; }
    }
}