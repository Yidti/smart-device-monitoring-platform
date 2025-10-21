using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDeviceMonitoring.Web.Models
{
    public class Alert
    {
        [Key]
        public long AlertId { get; set; }

        [Required]
        public int SensorId { get; set; }
        [ForeignKey("SensorId")]
        public Sensor Sensor { get; set; }

        [Required]
        [StringLength(50)]
        public string AlertType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TriggerValue { get; set; }

        [Required]
        public DateTime AlertTime { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsResolved { get; set; } = false;

        public DateTime? ResolvedTime { get; set; }

        public string Notes { get; set; }
    }
}
