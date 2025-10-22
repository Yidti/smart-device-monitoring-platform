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
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SensorData.Include(s => s.Sensor).ThenInclude(s => s.Device);
            return View(await applicationDbContext.ToListAsync());
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
                .FirstOrDefaultAsync(m => m.DataId == id);
            if (sensorData == null)
            {
                return NotFound();
            }

            return View(sensorData);
        }
    }
}
