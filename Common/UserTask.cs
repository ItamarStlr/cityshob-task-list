using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// a to-do list task.
    /// </summary>
    [DataContract]
    public class UserTask : INotifyPropertyChanged
    {
        private string _description;
        private bool _isCompleted;
        private bool _isLocked;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The task identifier.
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// The task description.
        /// </summary>
        [DataMember]
        public string Description 
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        /// <summary>
        /// Is the task marked as completed.
        /// </summary>
        [DataMember]
        public bool IsCompleted 
        {
            get
            {
                return _isCompleted;
            }
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged(nameof(IsCompleted));
                }
            }
        }

        /// <summary>
        /// Is the task locked for modification.
        /// </summary>
        [DataMember]
        public bool IsLocked
        {
            get
            {
                return _isLocked;
            }
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    OnPropertyChanged(nameof(IsLocked));
                }
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserTask()
        {
            Id = Guid.NewGuid();
            Description = string.Empty;
            IsCompleted = false;
            IsLocked = false;
        }

        /// <summary>
        /// Constructor with parameters.
        /// </summary>
        /// <param name="description">The task description.</param>
        /// <param name="isCompleted">Is the task marked as completed.</param>
        /// <param name="isLocked">Is the task locked for modification.</param>
        public UserTask (string description, bool isCompleted = false, bool isLocked = false)
        {
            Id = Guid.NewGuid();
            Description = description;
            IsCompleted = isCompleted;
            IsLocked = isLocked;
        }

        /// <summary>
        /// Handle property changes notification.
        /// </summary>
        /// <param name="propertyName">The changed property name.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
