using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartDeviceMonitoring.Web.Data;
using SmartDeviceMonitoring.Web.Models;

namespace SmartDeviceMonitoring.Web.Pages
{
    public class LegacyDeviceConfigModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LegacyDeviceConfigModel(ApplicationDbContext context)
        {
            _context = context;
            Device = new Device { DeviceName = string.Empty }; // Initialize DeviceName
            Devices = new List<Device>(); // Initialize Devices in constructor
        }

        [BindProperty]
        public Device? Device { get; set; }
        public List<Device> Devices { get; set; }

        public async Task OnGetAsync(int? id)
        {
            Devices = await _context.Devices.OrderBy(d => d.DeviceName).ToListAsync();

            if (id.HasValue)
            {
                Device = await _context.Devices.FirstOrDefaultAsync(d => d.DeviceId == id);
            }
            else
            {
                Device = new Device { DeviceName = string.Empty }; // Initialize DeviceName
            }
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (Device == null)
            {
                return NotFound();
            }

            var currentDevice = Device; // Assign to a non-nullable local variable

            if (!ModelState.IsValid)
            {
                Devices = await _context.Devices.OrderBy(d => d.DeviceName).ToListAsync();
                return Page();
            }

            if (currentDevice.DeviceId == 0)
            {
                // Create new device
                currentDevice.CreatedAt = DateTime.UtcNow;
                currentDevice.UpdatedAt = DateTime.UtcNow;
                _context.Devices.Add(currentDevice);
            }
            else
            {
                // Update existing device
                var deviceToUpdate = await _context.Devices.FindAsync(currentDevice.DeviceId);
                if (deviceToUpdate == null)
                {
                    return NotFound();
                }

                deviceToUpdate.DeviceName = currentDevice.DeviceName;
                deviceToUpdate.Location = currentDevice.Location;
                deviceToUpdate.Description = currentDevice.Description;
                deviceToUpdate.IsActive = currentDevice.IsActive;
                deviceToUpdate.UpdatedAt = DateTime.UtcNow;

                _context.Entry(deviceToUpdate).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./LegacyDeviceConfig");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var device = await _context.Devices.FindAsync(id);

            if (device != null)
            {
                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./LegacyDeviceConfig");
        }
    }
}
