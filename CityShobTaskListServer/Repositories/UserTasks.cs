namespace CityShobTaskListServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserTasks
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsLocked { get; set; }
    }
}
