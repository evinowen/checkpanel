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
            public List<SendDailyReportModelRecord> Records { get; set; }
        }

        public class SendDailyReportModelRecord
        {
            public string Name { get; set; }
            public List<string> Punches { get; set; }
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
            var report_to_email_address = _configuration["REPORT_TO_EMAIL_ADDRESS"];
            var report_to_name = _configuration["REPORT_TO_NAME"];

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
                    var records = new List<SendDailyReportModelRecord>();

                    {
                        var yesterday = DateTime.Now.AddDays(-1);

                        var items = _context.Items.Where(i => i.DeletedAt == null);

                        switch (yesterday.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                items = items.Where(i => i.Sunday == true);
                                break;
                            case DayOfWeek.Monday:
                                items = items.Where(i => i.Monday == true);
                                break;
                            case DayOfWeek.Tuesday:
                                items = items.Where(i => i.Tuesday == true);
                                break;
                            case DayOfWeek.Wednesday:
                                items = items.Where(i => i.Wednesday == true);
                                break;
                            case DayOfWeek.Thursday:
                                items = items.Where(i => i.Thursday == true);
                                break;
                            case DayOfWeek.Friday:
                                items = items.Where(i => i.Friday == true);
                                break;
                            case DayOfWeek.Saturday:
                                items = items.Where(i => i.Sunday == true);
                                break;
                        }


                        foreach (var item in items.ToList())
                        {
                            points_available++;

                            var punches = _context.Entry(item).Collection(p => p.Punches).Query().Where(p => p.CreatedAt.Date == yesterday.Date).ToList();

                            if (punches.Count > 0)
                            {
                                points_earned++;
                            }
                        }
                    }

                    {
                        var items = _context.Items.OrderBy(i => i.DueHour).ThenBy(i => i.DueMinute).Where(i => i.DeletedAt == null);
                        foreach (var item in items.ToList())
                        {
                            var record = new SendDailyReportModelRecord
                            {
                                Name = item.Summary,
                                Punches = new List<string>() { "free", "free", "free", "free", "free", "free", "free" }
                            };

                            for (var i = 0; i < 7; i++)
                            {
                                var date = DateTime.Now.AddDays(-7 + i).Date;

                                switch (date.DayOfWeek)
                                {
                                    case DayOfWeek.Sunday:
                                        if (item.Sunday == true)
                                        {
                                            record.Punches[i] = "miss";
                                        }
                                        break;
                                    case DayOfWeek.Monday:
                                        if (item.Monday == true)
                                        {
                                            record.Punches[i] = "miss";
                                        }
                                        break;
                                    case DayOfWeek.Tuesday:
                                        if (item.Tuesday == true)
                                        {
                                            record.Punches[i] = "miss";
                                        }
                                        break;
                                    case DayOfWeek.Wednesday:
                                        if (item.Wednesday == true)
                                        {
                                            record.Punches[i] = "miss";
                                        }
                                        break;
                                    case DayOfWeek.Thursday:
                                        if (item.Thursday == true)
                                        {
                                            record.Punches[i] = "miss";
                                        }
                                        break;
                                    case DayOfWeek.Friday:
                                        if (item.Friday == true)
                                        {
                                            record.Punches[i] = "miss";
                                        }
                                        break;
                                    case DayOfWeek.Saturday:
                                        if (item.Saturday == true)
                                        {
                                            record.Punches[i] = "miss";
                                        }
                                        break;
                                }

                                if (record.Punches[i] == "miss")
                                {
                                    var punches = _context.Entry(item).Collection(p => p.Punches).Query().OrderBy(p => p.CreatedAt).Where(p => p.CreatedAt.Date == date).ToList();

                                    if (punches.Count > 0)
                                    {
                                        record.Punches[i] = "hit";
                                    }

                                }

                            }

                            records.Add(record);
                        }
                    }

                    var data = new SendDailyReportModel
                    {
                        Name = report_to_name,
                        EmailAddress = report_to_email_address,
                        PointsEarned = points_earned,
                        PointsAvailable = points_available,
                        Records = records
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
