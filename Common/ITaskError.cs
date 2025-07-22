using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Interface for a server errors.
    /// </summary>
    public interface ITaskError
    {
        /// <summary>
        /// The error message.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// The exception that caused the error, or null if none available.
        /// </summary>
        Exception Exception { get; set; }
    }
}
