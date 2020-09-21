using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using checkpanel.Models;
using Microsoft.AspNetCore.Mvc;

namespace checkpanel.Controllers
{
    [Route("Login")]
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new LoginViewModel{ Code = 1000 };
            return View(model);
        }
    }
}