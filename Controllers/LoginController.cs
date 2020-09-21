using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using checkpanel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace checkpanel.Controllers
{
    [Route("Login")]
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new LoginViewModel { State = HttpContext.Session.GetString("State") };

            return View(model);
        }

        [HttpPost("generate")]
        public IActionResult Generate()
        {

            Random generator = new Random();
            string authentication = generator.Next(0, 999999).ToString("D6");

            HttpContext.Session.SetString("State", "Generated");
            HttpContext.Session.SetString("Authentication", authentication);

            var twilio_account_sid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var twilio_auth_token = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            var twilio_telephone_from = Environment.GetEnvironmentVariable("TWILIO_TELEPHONE_FROM");
            var twilio_telephone_to = Environment.GetEnvironmentVariable("TWILIO_TELEPHONE_TO");

            TwilioClient.Init(twilio_account_sid, twilio_auth_token);

            var message = MessageResource.Create(
                body: $"checkpanel: {authentication}",
                from: new Twilio.Types.PhoneNumber(twilio_telephone_from),
                to: new Twilio.Types.PhoneNumber(twilio_telephone_to)
            );

            return LocalRedirect("/Login");
        }

        [HttpPost("validate")]
        public IActionResult Validate(string authentication)
        {
            if (authentication == HttpContext.Session.GetString("Authentication"))
            {
                HttpContext.Session.SetString("State", "Authenticated");
                return LocalRedirect("/List");
            }

            return LocalRedirect("/Login");
        }
    }
}