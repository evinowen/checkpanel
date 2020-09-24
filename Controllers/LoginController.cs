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

        [HttpPost("generate")]
        public async Task<IActionResult> Generate()
        {

            Random generator = new Random();
            string authentication = generator.Next(0, 999999).ToString("D6");

            HttpContext.Session.SetString("State", "Generated");
            HttpContext.Session.SetString("Authentication", authentication);

            var twilio_telephone_to = _configuration["TWILIO_TELEPHONE_TO"];

            var queue_connection_string = _configuration["AZURE_QUEUE_CONNECTION"];
            var queue_name = _configuration["AZURE_QUEUE_SEND_AUTHENTICATION_CODE"];

            QueueClient queue = new QueueClient(queue_connection_string, queue_name);

            var data = new SendAuthenticationCodeModel
            {
                Authentication = authentication,
                Telephone = twilio_telephone_to
            };

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(data, options);
            Message message = new Message(Encoding.ASCII.GetBytes(json));

            await queue.SendAsync(message);

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