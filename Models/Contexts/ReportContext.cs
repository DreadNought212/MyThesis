using Artium.Models.Objects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artium.Models.Contexts
{
    public class ReportContext : DbContext
    {
        public DbSet<WallPost> WallPosts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public ReportContext(DbContextOptions<ReportContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
