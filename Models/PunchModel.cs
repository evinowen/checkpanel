using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace checkpanel.Models
{
    public class PunchModel
    {
        public int Id { get; set; }
        public ItemModel Item { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}