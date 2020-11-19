using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace checkpanel.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }
        public int DueHour { get; set; }
        public int DueMinute { get; set; }
        public bool? Sunday { get; set; }
        public bool? Monday { get; set; }
        public bool? Tuesday { get; set; }
        public bool? Wednesday { get; set; }
        public bool? Thursday { get; set; }
        public bool? Friday { get; set; }
        public bool? Saturday { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        [JsonIgnore]
        public List<PunchModel> Punches { get; set; }

        public HistoryModel Record(string Summary, string Detail = "")
        {
            HistoryModel history = new HistoryModel();

            history.Item = this;
            history.Summary = Summary;
            history.Detail = Detail;
            history.RecordedAt = DateTime.Now;

            return history;
        }
    }
}