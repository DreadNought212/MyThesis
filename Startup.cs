using Artium.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Artium.Models.Contexts;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Artium
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<UserContext>(optionsBuilder => optionsBuilder.UseMySql(
                connection,
                new MySqlServerVersion(new Version(8, 0, 11))));
            services.AddDbContext<PicUserContext>(optionsBuilder => optionsBuilder.UseMySql(
                connection,
                new MySqlServerVersion(new Version(8, 0, 11))));
            services.AddDbContext<WallPostContext>(optionsBuilder => optionsBuilder.UseMySql(
               connection,
                new MySqlServerVersion(new Version(8, 0, 11))));
            services.AddDbContext<CommentContext>(optionsBuilder => optionsBuilder.UseMySql(
               connection,
                new MySqlServerVersion(new Version(8, 0, 11))));
            services.AddDbContext<ReportContext>(optionsBuilder => optionsBuilder.UseMySql(
               connection,
                new MySqlServerVersion(new Version(8, 0, 11))));
            services.AddDbContext<FollowingContext>(optionsBuilder => optionsBuilder.UseMySql(
               connection,
                new MySqlServerVersion(new Version(8, 0, 11))));
            services.AddDbContext<AdminContext>(optionsBuilder => optionsBuilder.UseMySql(
               connection,
                new MySqlServerVersion(new Version(8, 0, 11))));

            // ��������� ������������ �����������
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();    // ��������������
            app.UseAuthorization();     // �����������

            var routeBuilder = new RouteBuilder(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
