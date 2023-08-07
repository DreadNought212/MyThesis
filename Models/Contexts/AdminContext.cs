using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Artium.Models.Objects;
using Microsoft.EntityFrameworkCore;

namespace Artium.Models.Contexts
{
    public class AdminContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public AdminContext(DbContextOptions<AdminContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
