using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERManager.Models;

namespace ERManager.Data
{
    public class ERManagerContext : DbContext
    {
        public ERManagerContext (DbContextOptions<ERManagerContext> options)
            : base(options)
        {
        }

        public DbSet<ERManager.Models.Branch> Branch { get; set; } = default!;
    }
}
