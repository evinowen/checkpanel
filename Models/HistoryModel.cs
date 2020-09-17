using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace checkpanel.Models
{
    public class HistoryModel
    {
        public int Id { get; set; }
        public ItemModel Item { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }
        public string BeforeJSON { get; set; }
        public string AfterJSON { get; set; }
        public DateTime RecordedAt { get; set; }

        public void CaptureBeforeData(ItemModel item)
        {
            this.BeforeJSON = JsonSerializer.Serialize(item);
        }

        public void CaptureAfterData(ItemModel item)
        {
            this.AfterJSON = JsonSerializer.Serialize(item);
        }
    }
}