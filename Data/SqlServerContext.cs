using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using checkpanel.Models;

namespace checkpanel.Data
{
    public class SqlServerContext : DbContext
    {
        public SqlServerContext(DbContextOptions<SqlServerContext> options) 
            : base(options)
        {
        }

        public DbSet<ItemModel> Items { get; set; }
        public DbSet<PunchModel> Punches { get; set; }
        public DbSet<HistoryModel> Histories { get; set; }
    }
}
