using Artium.Models.Contexts;
using Artium.Models.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artium.Controllers
{
    public class FeedController : Controller
    {
        private UserContext db_user;
        private PicUserContext db_userpic;
        private WallPostContext db_post;
        private CommentContext db_comments;
        IWebHostEnvironment _appEnvironment;
        private AdminContext db_admins;
        public FeedController(UserContext user_context, 
            PicUserContext userpics_context, 
            IWebHostEnvironment appEnvironment, 
            WallPostContext post_context,
            CommentContext comment_context,
            AdminContext admins_context)
        {
            db_user = user_context;
            db_userpic = userpics_context;
            db_post = post_context;
            _appEnvironment = appEnvironment;
            db_comments = comment_context;
            db_admins = admins_context;
        }

        [Authorize]
        public async Task<IActionResult> Index(int id)
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
            }
            else
            {
                ViewBag.isAdmin = false;
            }

            ViewBag.user = user;

            ViewBag.WallPosts = db_post.WallPosts
                        .Where(u => u.Disabled == 0)
                        .Where(u => u.UserId != user.Id)
                        .Include(u => u.User)
                        .Include(u => u.User.UserInfo)
                        .Include(u => u.User.UserInfo.Bguserpic)
                        .OrderByDescending(s => s.Date)
                        .Include(u => u.User.UserInfo.Userpic);

            return View();
        }
    }
}
