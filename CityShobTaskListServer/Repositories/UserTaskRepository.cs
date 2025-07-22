using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityShobTaskListServer.Repositories
{
    public class UserTaskRepository : IUserTaskRepository
    {
        // Manages user tasks in memory.
        private ConcurrentDictionary<Guid, Common.UserTask> _userTasks = new ConcurrentDictionary<Guid, Common.UserTask>();

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
        /// <param name="dbContext">The DB context.</param>
        /// <param name="logger">The logger.</param>
        public UserTaskRepository(ILogger<UserTaskRepository> logger)
        {
            Logger = logger;

            Initialize();

            Logger.LogInformation("UserTaskRepository initialized.");
        }

        /// <summary>
        /// Initialize the repository.
        /// </summary>
        public void Initialize()
        {
            var initialTasks = new List<UserTasks>();

            using (var dbContext = new UserTasksDbContext())
            {
                try
                {
                    initialTasks = dbContext.UserTasks.AsNoTracking().ToList();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error initializing user task repository.");
                    throw new InvalidOperationException("Error initializing user task repository.", ex);
                }
            }

            if (initialTasks != null && initialTasks.Any())
            {
                foreach (var taskDto in initialTasks)
                {
                    if (taskDto != null && taskDto.Id != Guid.Empty)
                    {
                        var userTask = ConvertToModel(taskDto);
                        _userTasks.TryAdd(userTask.Id, userTask);
                    }
                    else
                    {
                        Logger.LogError($"Invalid user task in DB tasks.");
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new user task to the repository.
        /// </summary>
        /// <param name="userTask">The task to add.</param>
        /// <returns></returns>
        public async Task<bool> AddTaskAsync(Common.UserTask userTask)
        {
            // Add user task to the in-memory repository.
            try
            {
                if (userTask == null || userTask.Id == Guid.Empty)
                {
                    Logger.LogError("Invalid user task.");
                    throw new ArgumentException("Invalid user task.");
                }

                // Add to in-memory repository.
                if (_userTasks.TryAdd(userTask.Id, userTask) == false)
                {
                    Logger.LogError($"User task with ID {userTask.Id} already exists in repository.");
                    throw new InvalidOperationException($"User task with ID {userTask.Id} already exists in repository.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error adding user task to repository: {ex.Message}");
                throw ex;
            }

            // Add user task to the database.
            using (var dbContext = new UserTasksDbContext())
            {
                try
                {
                    dbContext.UserTasks.Add(ConvertToDto(userTask));
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error saving user task to database: {ex.Message}");
                    throw new InvalidOperationException("Error saving user task to database.", ex);
                }
            }

            UserTaskAdded?.Invoke(this, new UserTaskEventArgs { Id = userTask.Id, UserTask = userTask });

            return true;
        }

        /// <summary>
        /// Deletes an existing user task from the repository.
        /// </summary>
        /// <param name="taskId">The task to delete.</param>
        /// <returns></returns>
        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {
            Common.UserTask deletedTask;

            // Remove user task from the in-memory repository.
            try
            {
                if (taskId == Guid.Empty)
                {
                    Logger.LogError("Invalid user task ID.");
                    throw new ArgumentException("Invalid user task ID.");
                }

                if (_userTasks.TryRemove(taskId, out deletedTask) == false)
                {
                    Logger.LogError($"User task with ID {taskId} not found in repository.");
                    throw new KeyNotFoundException($"User task with ID {taskId} not found in repository.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error removing user task from repository: {ex.Message}");
                throw ex;
            }

            // Remove user task from the database.
            using (var dbContext = new UserTasksDbContext())
            {
                try
                {
                    var existing = await dbContext.UserTasks.FindAsync(taskId);
                    if (existing == null)
                    {
                        Logger.LogError($"Error removing user task from database.");
                        throw new InvalidOperationException($"Error removing user task from database.");
                    }

                    dbContext.UserTasks.Remove(existing);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error removing user task from database: {ex.Message}");
                    throw new InvalidOperationException("Error removing user task from database.", ex);
                }
            }

            UserTaskDeleted?.Invoke(this, new UserTaskEventArgs { Id = taskId, UserTask = deletedTask });

            return true;
        }

        /// <summary>
        /// Updates an existing user task in the repository.
        /// </summary>
        /// <param name="userTask">The task to update.</param>
        /// <returns></returns>
        /// <remarks>Assumes user task locking is enforced on the client side.</remarks>
        public async Task<bool> UpdateTaskAsync(Common.UserTask userTask)
        {
            // Update user task in the in-memory repository.
            try
            {
                if (userTask == null || userTask.Id == Guid.Empty)
                {
                    Logger.LogError("Invalid user task.");
                    throw new ArgumentException("Invalid user task.");
                }

                _userTasks[userTask.Id] = userTask;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error updating user task in repository: {ex.Message}");
                throw new InvalidOperationException($"Error updating user task in repository: { ex.Message }");
            }

            // Update user task in the database.
            using (var dbContext = new UserTasksDbContext())
            {
                try
                {
                    var existing = await dbContext.UserTasks.FindAsync(userTask.Id);
                    if (existing == null)
                    {
                        Logger.LogError($"Error updating user task in database.");
                        throw new InvalidOperationException($"Error updating user task in database.");
                    }

                    // Update exisying dto
                    existing.Description = userTask.Description;
                    existing.IsCompleted = userTask.IsCompleted;
                    existing.IsLocked = userTask.IsLocked;

                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Error saving user task to database: {ex.Message}");
                    throw new InvalidOperationException("Error saving user task to database.", ex);
                }
            }

            UserTaskUpdated?.Invoke(this, new UserTaskEventArgs { Id = userTask.Id, UserTask = userTask });

            return true;
        }

        /// <summary>
        /// Gets a user task by its ID.
        /// </summary>
        /// <param name="taskId">The task ID.</param>
        /// <returns>The requested user task or null if none exists.</returns>
        public async Task<Common.UserTask> GetTaskAsync(Guid taskId)
        {
            try
            {
                if (taskId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid user task.");
                }

                Common.UserTask userTask;
                if (_userTasks.TryGetValue(taskId, out userTask))
                {
                    return userTask;
                }
                else
                {
                    Logger?.LogError($"Failed to retrieve user task from repository: {taskId}");
                    throw new KeyNotFoundException($"User task with ID {taskId} not found.");
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error retrieving user task from repository: {ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// Gets all user tasks from the repository.
        /// </summary>
        /// <returns>All user tasks in the repository.</returns>
        public async Task<IEnumerable<Common.UserTask>> GetAllTasksAsync()
        {
            try
            {
                return _userTasks.Values.AsEnumerable();
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, $"Error retrieving all user tasks from repository: {ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// Convert user task to DB data transfer object.
        /// </summary>
        /// <param name="userTask">The user task.</param>
        /// <returns></returns>
        private UserTasks ConvertToDto(Common.UserTask userTask)
        {
            return new UserTasks
            {
                Id = userTask.Id,
                Description = userTask.Description,
                IsCompleted = userTask.IsCompleted,
                IsLocked = userTask.IsLocked
            };
        }

        /// <summary>
        /// Convert to user task from DB data transfer object.
        /// </summary>
        /// <param name="userTaskDto">The task DTO.</param>
        /// <returns></returns>
        private Common.UserTask ConvertToModel(UserTasks userTaskDto)
        {
            return new Common.UserTask
            {
                Id = userTaskDto.Id,
                Description = userTaskDto.Description,
                IsCompleted = userTaskDto.IsCompleted,
                IsLocked = userTaskDto.IsLocked
            };
        }
    }
}
