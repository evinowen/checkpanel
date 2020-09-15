using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace checkpanel.Models
{
    public class HistoryModel
    {
        public int Id { get; set; }
        public ItemModel Item { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}