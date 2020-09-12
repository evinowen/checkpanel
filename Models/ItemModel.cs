using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace checkpanel.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }
    }
}