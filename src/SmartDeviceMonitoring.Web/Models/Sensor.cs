using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDeviceMonitoring.Web.Models
{
    public class Sensor
    {
        [Key]
        public int SensorId { get; set; }

        [Required]
        [StringLength(255)]
        public string SensorName { get; set; } = string.Empty; // Initialized

        [Required]
        public int DeviceId { get; set; }
        [ForeignKey("DeviceId")]
        public Device? Device { get; set; } // Made nullable

        [Required]
        public int SensorTypeId { get; set; }
        [ForeignKey("SensorTypeId")]
        public SensorType? SensorType { get; set; } // Made nullable

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? MinThreshold { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? MaxThreshold { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<SensorData>? SensorData { get; set; } // Made nullable
        public ICollection<Alert>? Alerts { get; set; } // Made nullable
    }
}