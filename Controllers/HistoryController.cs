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
        public IActionResult Index()
        {
            return View(_context.Histories.ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
