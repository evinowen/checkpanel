using System;
using System.Collections.Generic;
using System.Linq;
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
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public List<PunchModel> Punches { get; set; }
    }
}