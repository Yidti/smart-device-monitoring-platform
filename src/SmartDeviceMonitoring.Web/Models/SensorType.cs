using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SmartDeviceMonitoring.Web.Models
{
    public class SensorType
    {
        [Key]
        public int SensorTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string TypeName { get; set; } = string.Empty; // Initialized

        [StringLength(50)]
        public string? Unit { get; set; } // Made nullable

        public string? Description { get; set; } // Made nullable

        // Navigation property
        public ICollection<Sensor>? Sensors { get; set; } // Made nullable
    }
}