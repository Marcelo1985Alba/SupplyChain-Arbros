using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.Context
{
    public class GanttContext : DbContext
    {
        public DbSet<GanttDataDetails> GanttData { get; set; }

        public GanttContext(DbContextOptions<GanttContext> options)
            : base(options) { }
    }
}
