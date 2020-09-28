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
    public class DeadlineController : ControllerBase
    {
        public class Deadline
        {
            public int DueHour { get; set; }
            public int DueMinute { get; set; }
        }

        public class SendDeadlineNotice
        {
            public string Summary { get; set; }
            public int Deadline { get; set; }
            public string Telephone { get; set; }
        }

        private readonly IConfiguration _configuration;
        private readonly ILogger<ListController> _logger;
        private readonly SqlServerContext _context;

        public DeadlineController(ILogger<ListController> logger, IConfiguration configuration, SqlServerContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Deadline deadline)
        {
            var deadline_minutes = (deadline.DueHour * 60) + deadline.DueMinute;

            var items = _context.Items
                .Where(i => i.DeletedAt == null)
                .Where(i => (i.DueHour * 60) + i.DueMinute <= deadline_minutes)
                .Where(i => (i.DueHour * 60) + i.DueMinute > deadline_minutes - 60)
                .ToList();

            foreach (var item in items)
            {
                var punches = _context.Entry(item).Collection(p => p.Punches).Query().Where(p => p.CreatedAt.Date == DateTime.Today).ToList();

                if (punches.Count > 0)
                {
                    continue;
                }

                var twilio_telephone_to = _configuration["TWILIO_TELEPHONE_TO"];

                var queue_connection_string = _configuration["AZURE_QUEUE_CONNECTION"];
                var queue_name = _configuration["AZURE_QUEUE_SEND_DEADLINE_NOTICE"];

                QueueClient queue = new QueueClient(queue_connection_string, queue_name);

                var data = new SendDeadlineNotice
                {
                    Summary = item.Summary,
                    Deadline = ((item.DueHour * 60) + item.DueMinute - deadline_minutes) * -1,
                    Telephone = twilio_telephone_to
                };

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var json = JsonSerializer.Serialize(data, options);
                Message message = new Message(Encoding.ASCII.GetBytes(json));

                await queue.SendAsync(message);
            }

            return Ok();
        }
    }
}