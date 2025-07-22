using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class UserTaskEventArgs : EventArgs
    {
        /// <summary>
        /// The user task ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The user task.
        /// </summary>
        public UserTask UserTask { get; set; }
    }
}
