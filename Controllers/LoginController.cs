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

            Random generator = new Random();
            string authentication = generator.Next(0, 999999).ToString("D6");

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

            var model = new LoginViewModel { Authentication = authentication };

            return View(model);
        }
    }
}