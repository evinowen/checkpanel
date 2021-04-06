using System;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using checkpanel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using OtpNet;

namespace checkpanel.Controllers
{
    [Route("Login")]
    public class LoginController : Controller
    {
        public class SendAuthenticationCodeModel
        {
            public string Authentication { get; set; }
            public string Telephone { get; set; }
        }

        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new LoginViewModel { State = HttpContext.Session.GetString("State") };

            return View(model);
        }

        [HttpPost("validate")]
        public IActionResult Validate(string authentication)
        {
            var secret = _configuration["TOTP_SECRET"];

            var base32Bytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(base32Bytes);
            var code = totp.ComputeTotp();

            if (authentication == code)
            {
                HttpContext.Session.SetString("State", "Authenticated");
                return LocalRedirect("/List");
            }

            return LocalRedirect("/Login");
        }
    }
}