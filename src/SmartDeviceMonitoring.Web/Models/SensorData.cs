using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDeviceMonitoring.Web.Models
{
    public class SensorData
    {
        [Key]
        public long DataId { get; set; }

        [Required]
        public int SensorId { get; set; }
        [ForeignKey("SensorId")]
        public Sensor Sensor { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Value { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
