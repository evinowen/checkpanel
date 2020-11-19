using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using checkpanel.Models;
using checkpanel.Data;

namespace checkpanel.Controllers
{

    [Route("Configuration")]
    public class ConfigurationController : Controller
    {
        private readonly ILogger<ConfigurationController> _logger;
        private readonly SqlServerContext _context;

        public ConfigurationController(ILogger<ConfigurationController> logger, SqlServerContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Items.Where(i => i.DeletedAt == null).ToList());
        }

        [HttpPost]
        public IActionResult AddItem(int id, string Summary, string Detail, int DueHour, int DueMinute, string Sunday, string Monday, string Tuesday, string Wednesday, string Thursday, string Friday, string Saturday)
        {
            var item = new ItemModel();

            var history = item.Record("Configuration");

            item.Summary = Summary;
            item.Detail = Detail;
            item.DueHour = DueHour;
            item.DueMinute = DueMinute;

            item.Sunday = Sunday == "on";
            item.Monday = Monday == "on";
            item.Tuesday = Tuesday == "on";
            item.Wednesday = Wednesday == "on";
            item.Thursday = Thursday == "on";
            item.Friday = Friday == "on";
            item.Saturday = Saturday == "on";

            item.ModifiedAt = DateTime.Now;

            history.CaptureAfterData(item);

            _context.Items.Add(item);
            _context.Histories.Add(history);

            _context.SaveChanges();

            return LocalRedirect("/Configuration");
        }

        [HttpPost("{id}")]
        public IActionResult UpdateItem(int id, string Summary, string Detail, int DueHour, int DueMinute, string Sunday, string Monday, string Tuesday, string Wednesday, string Thursday, string Friday, string Saturday, string Delete)
        {
            var item = _context.Items.FirstOrDefault(item => item.Id == id);

            if (item != null)
            {
                var history = item.Record("Configuration");
                history.CaptureBeforeData(item);

                item.ModifiedAt = DateTime.Now;

                item.Summary = Summary;
                item.Detail = Detail;
                item.DueHour = DueHour;
                item.DueMinute = DueMinute;

                item.Sunday = Sunday == "on";
                item.Monday = Monday == "on";
                item.Tuesday = Tuesday == "on";
                item.Wednesday = Wednesday == "on";
                item.Thursday = Thursday == "on";
                item.Friday = Friday == "on";
                item.Saturday = Saturday == "on";

                if (Delete == "on")
                {
                    item.DeletedAt = DateTime.Now;
                }

                history.CaptureAfterData(item);

                _context.Histories.Add(history);

                _context.SaveChanges();
            }

            return LocalRedirect("/Configuration");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
