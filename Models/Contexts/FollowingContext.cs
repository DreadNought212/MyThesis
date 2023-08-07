using Artium.Models.Objects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artium.Models.Contexts
{
    public class FollowingContext : DbContext
    {
        public DbSet<UserFollower> Following { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Userpic> Userpics { get; set; }
        public DbSet<Bguserpic> Bguserpics { get; set; }
        public FollowingContext(DbContextOptions<FollowingContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
