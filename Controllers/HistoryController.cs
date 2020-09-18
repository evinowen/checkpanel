using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using checkpanel.Models;
using checkpanel.Data;

namespace checkpanel.Controllers
{
    [Route("History")]
    public class HistoryController : Controller
    {
        private readonly ILogger<HistoryController> _logger;
        private readonly SqlServerContext _context;

        public HistoryController(ILogger<HistoryController> logger, SqlServerContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [HttpGet("{Page}")]
        public IActionResult Index(int Page = 1)
        {
            var model = new HistoryViewModel();

            model.CurrentPage = Page;

            model.TotalItems = _context.Histories.Count();
            model.TotalPages = (int) Math.Ceiling(((double) model.TotalItems) / ((double) model.ItemsPerPage));

            model.Records = _context.Histories.OrderByDescending(h => h.RecordedAt).Skip(model.ItemsPerPage * (model.CurrentPage - 1)).Take(model.ItemsPerPage).ToList();

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
