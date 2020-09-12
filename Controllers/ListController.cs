using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using checkpanel.Data;
using checkpanel.Models;

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
            return View(_context.Items.ToList());
        }

        [HttpPost("{id}")]
        public IActionResult PunchItem(int id)
        {
            return LocalRedirect("/List");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
