using Artium.Models;
using Artium.Models.Contexts;
using Artium.Models.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Artium.Controllers
{
    public class ReportController : Controller
    {
        private UserContext db_user;
        private PicUserContext db_userpic;
        private WallPostContext db_post;
        private FollowingContext db_followers;
        private AdminContext db_admins;
        private ReportContext db_report;
        IWebHostEnvironment _appEnvironment;
        public ReportController(UserContext user_context,
            PicUserContext userpics_context,
            IWebHostEnvironment appEnvironment,
            WallPostContext post_context,
            AdminContext admins_context,
            FollowingContext followers_context,
            ReportContext reportContext
            )
        {
            db_user = user_context;
            db_userpic = userpics_context;
            db_post = post_context;
            db_followers = followers_context;
            _appEnvironment = appEnvironment;
            db_admins = admins_context;
            db_report = reportContext;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.isAuth = true;
            }
            else
            {
                ViewBag.isAuth = false;
            }

            User user = await db_user.Users
                    .Include(u => u.UserInfo)
                    .Include(u => u.UserInfo.Bguserpic)
                    .Include(u => u.UserInfo.Userpic).FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            ViewBag.Lang = user.UserInfo.Lang;

            ViewBag.Theme = user.UserInfo.Theme;

            Admin admin = await db_admins.Admins.FirstOrDefaultAsync(u => u.UserId == user.Id);


                if (admin != null)
                {
                ViewBag.isAdmin = true;

                ViewBag.user = user;

                ViewBag.Reports = db_report.Reports
                    .Where(u => u.Resolved == 0)
                    .Include(u => u.WallPost)
                    .Include(u => u.WallPost.User)
                    .OrderByDescending(s => s.Date);

                return View();
                }
                else
                {
                ViewBag.isAdmin = false;
                return LocalRedirect("~/" + User.Identity.Name);
                }
        }

        [HttpPost]
        public async Task<IActionResult> NewPostReport(string reason, string text, int WallPostId)
        {
            User user = await db_report.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            WallPost wallPost = await db_report.WallPosts.Include(p => p.User)
                       .Include(p => p.User.UserInfo)
                       .FirstOrDefaultAsync(p => p.Id == WallPostId);

            if (user != null)
            {
                if (text != null && reason != null && WallPostId != 0)
                {
                    Report report = new Report { Date = DateTime.Now, Reason = reason, Resolved = 0, Text = text, WallPostId = WallPostId };

                    db_report.Reports.Add(report);
                    db_report.SaveChanges();

                    return LocalRedirect("~/" + wallPost.User.Login);
                };
            }
            return LocalRedirect("~/" + User.Identity.Name);
        }
        [HttpPost]
        public async Task<IActionResult> ResolveReport(int id)
        {
            Report report = await db_report.Reports.FirstOrDefaultAsync(u => u.Id == id);

            if (report != null)
            {
                report.Resolved = 1;
                db_report.Reports.Update(report);
                db_report.SaveChanges();
            }
            return LocalRedirect("~/Report");
        }
       
    }
}
