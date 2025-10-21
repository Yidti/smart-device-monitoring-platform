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
        public string TypeName { get; set; }

        [StringLength(50)]
        public string Unit { get; set; }

        public string Description { get; set; }

        // Navigation property
        public ICollection<Sensor> Sensors { get; set; }
    }
}
