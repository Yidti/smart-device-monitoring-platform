using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDeviceMonitoring.Web.Models
{
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }

        [Required]
        [StringLength(255)]
        public required string DeviceName { get; set; } // Added 'required' keyword

        [StringLength(255)]
        public string? Location { get; set; }

        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Sensor>? Sensors { get; set; }
    }
}
