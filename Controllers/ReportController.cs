using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using checkpanel.Data;
using checkpanel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace checkpanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        public class Report
        {
            public string Name { get; set; }
        }

        public class SendDailyReportModel
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
            public int PointsEarned { get; set; }
            public int PointsAvailable { get; set; }
        }

        private readonly IConfiguration _configuration;
        private readonly ILogger<ListController> _logger;
        private readonly SqlServerContext _context;

        public ReportController(ILogger<ListController> logger, IConfiguration configuration, SqlServerContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Report report)
        {
            var sendgrid_email_address_to = _configuration["SENDGRID_EMAIL_ADDRESS_TO"];
            var sendgrid_name = _configuration["SENDGRID_NAME"];

            string queue_connection_string = _configuration["AZURE_QUEUE_CONNECTION"];
            string queue_name;

            string json;

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            switch (report.Name)
            {
                case "daily":
                    var points_earned = 0;
                    var points_available = 0;

                    foreach (var item in _context.Items.Where(i => i.DeletedAt == null).ToList())
                    {
                        points_available++;

                        var punches = _context.Entry(item).Collection(p => p.Punches).Query().Where(p => p.CreatedAt.Date == DateTime.Now.Date.AddDays(-1)).ToList();

                        if (punches.Count > 0)
                        {
                            points_earned++;
                        }
                    }

                    var data = new SendDailyReportModel{
                        Name = sendgrid_name,
                        EmailAddress = sendgrid_email_address_to,
                        PointsEarned = points_earned,
                        PointsAvailable = points_available
                    };

                    queue_name = _configuration["AZURE_QUEUE_SEND_DAILY_REPORT"];
                    json = JsonSerializer.Serialize(data, options);
                    break;

                default:
                    return BadRequest();
            }

            Message message = new Message(Encoding.ASCII.GetBytes(json));

            QueueClient queue = new QueueClient(queue_connection_string, queue_name);
            await queue.SendAsync(message);

            return Ok();
        }
    }
}
