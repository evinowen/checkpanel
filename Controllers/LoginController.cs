using System;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using checkpanel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Queues;

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

            var twilio_telephone_to = Environment.GetEnvironmentVariable("TWILIO_TELEPHONE_TO");

            var queue_connection_string = Environment.GetEnvironmentVariable("AZURE_QUEUE_CONNECTION");
            var queue_name = Environment.GetEnvironmentVariable("AZURE_QUEUE_SEND_AUTHENTICATION_CODE");

            QueueClient queue = new QueueClient(queue_connection_string, queue_name);

            var data = new SendAuthenticationCodeModel
            {
                Authentication = authentication,
                Telephone = twilio_telephone_to
            };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            queue.SendMessage(JsonSerializer.Serialize(data));

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