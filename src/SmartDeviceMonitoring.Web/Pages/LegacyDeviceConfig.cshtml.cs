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
        }

        [BindProperty]
        public Device Device { get; set; }
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
                Device = new Device(); // Initialize for new entry or no selection
            }
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                Devices = await _context.Devices.OrderBy(d => d.DeviceName).ToListAsync();
                return Page();
            }

            if (Device.DeviceId == 0)
            {
                // Create new device
                Device.CreatedAt = DateTime.UtcNow;
                Device.UpdatedAt = DateTime.UtcNow;
                _context.Devices.Add(Device);
            }
            else
            {
                // Update existing device
                var deviceToUpdate = await _context.Devices.FindAsync(Device.DeviceId);
                if (deviceToUpdate == null)
                {
                    return NotFound();
                }

                deviceToUpdate.DeviceName = Device.DeviceName;
                deviceToUpdate.Location = Device.Location;
                deviceToUpdate.Description = Device.Description;
                deviceToUpdate.IsActive = Device.IsActive;
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
