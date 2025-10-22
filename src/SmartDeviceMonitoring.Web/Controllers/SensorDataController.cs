using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartDeviceMonitoring.Web.Data;
using SmartDeviceMonitoring.Web.Models;

namespace SmartDeviceMonitoring.Web.Controllers
{
    public class SensorDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SensorDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SensorData
        public async Task<IActionResult> Index(int? deviceId, int? sensorTypeId, DateTime? startDate, DateTime? endDate)
        {
            IQueryable<SensorData> sensorData = _context.SensorData
                .Include(s => s.Sensor)
                .ThenInclude(s => s.Device)
                .Include(s => s.Sensor.SensorType); // Include SensorType for filtering

            // Filter by Device
            if (deviceId.HasValue)
            {
                sensorData = sensorData.Where(sd => sd.Sensor.DeviceId == deviceId.Value);
            }

            // Filter by SensorType
            if (sensorTypeId.HasValue)
            {
                sensorData = sensorData.Where(sd => sd.Sensor.SensorTypeId == sensorTypeId.Value);
            }

            // Filter by Date Range
            if (startDate.HasValue)
            {
                sensorData = sensorData.Where(sd => sd.Timestamp >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                sensorData = sensorData.Where(sd => sd.Timestamp <= endDate.Value);
            }

            // Populate ViewBag for dropdowns
            ViewBag.Devices = new SelectList(await _context.Devices.OrderBy(d => d.DeviceName).ToListAsync(), "DeviceId", "DeviceName", deviceId);
            ViewBag.SensorTypes = new SelectList(await _context.SensorTypes.OrderBy(st => st.TypeName).ToListAsync(), "SensorTypeId", "TypeName", sensorTypeId);

            return View(await sensorData.OrderByDescending(sd => sd.Timestamp).ToListAsync());
        }

        // GET: SensorData/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sensorData = await _context.SensorData
                .Include(s => s.Sensor)
                .ThenInclude(s => s.Device)
                .Include(s => s.Sensor.SensorType) // Include SensorType for display
                .FirstOrDefaultAsync(m => m.DataId == id);
            if (sensorData == null)
            {
                return NotFound();
            }

            return View(sensorData);
        }
    }
}