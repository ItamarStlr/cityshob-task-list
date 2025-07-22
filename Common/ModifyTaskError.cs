using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// An error that occurs during task modification operations.
    /// </summary>
    [DataContract]
    public class ModifyTaskError : ITaskError
    {
        /// <summary>
        /// The error message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// The exception that caused the error, or null if none available.
        /// </summary>
        [DataMember]
        public Exception Exception { get; set; }
    }
}
