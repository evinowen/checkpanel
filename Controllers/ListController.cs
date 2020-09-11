using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using checkpanel.Models;

namespace checkpanel.Controllers
{
    [Route("List")]
    public class ListController : Controller
    {
        private readonly ILogger<ListController> _logger;

        public ListController(ILogger<ListController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
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
