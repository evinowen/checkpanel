using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using checkpanel.Data;
using checkpanel.Models;
using Microsoft.EntityFrameworkCore;

namespace checkpanel.Controllers
{
    [Route("List")]
    public class ListController : Controller
    {
        private readonly ILogger<ListController> _logger;
        private readonly SqlServerContext _context;

        public ListController(ILogger<ListController> logger, SqlServerContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ListViewModel();
            var items = _context.Items.Where(i => i.DeletedAt == null);

            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    items = items.Where(i => i.Sunday == true);
                    break;
                case DayOfWeek.Monday:
                    items = items.Where(i => i.Monday == true);
                    break;
                case DayOfWeek.Tuesday:
                    items = items.Where(i => i.Tuesday == true);
                    break;
                case DayOfWeek.Wednesday:
                    items = items.Where(i => i.Wednesday == true);
                    break;
                case DayOfWeek.Thursday:
                    items = items.Where(i => i.Thursday == true);
                    break;
                case DayOfWeek.Friday:
                    items = items.Where(i => i.Friday == true);
                    break;
                case DayOfWeek.Saturday:
                    items = items.Where(i => i.Sunday == true);
                    break;
            }

            foreach (var item in items.ToList())
            {
                var punches = _context.Entry(item).Collection(p => p.Punches).Query().Where(p => p.CreatedAt.Date == DateTime.Now.Date).ToList();
                model.ListSet.Add((item, punches.Count > 0 ? punches.First() : null));
            }

            return View(model);
        }

        [HttpPost("{id}")]
        public IActionResult PunchItem(int id)
        {
            var item = _context.Items.FirstOrDefault(item => item.Id == id);

            if (item != null)
            {
                var history = item.Record("Punch");
                history.CaptureBeforeData(item);
                _context.Histories.Add(history);

                var punch = new PunchModel{ Item = item, CreatedAt = DateTime.Now };
                _context.Punches.Add(punch);

                _context.SaveChanges();
            }

            return LocalRedirect("/List");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
