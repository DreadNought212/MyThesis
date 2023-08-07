﻿using Artium.Models;
using Artium.Models.Contexts;
using Artium.Models.Objects;
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
    public class UserController : Controller
    {
        private UserContext db_user;
        private PicUserContext db_userpic;
        private WallPostContext db_post;
        private FollowingContext db_followers;
        private AdminContext db_admins;
        IWebHostEnvironment _appEnvironment;
        public UserController(UserContext user_context,
            PicUserContext userpics_context,
            IWebHostEnvironment appEnvironment,
            WallPostContext post_context,
            AdminContext admins_context,
            FollowingContext followers_context)
        {
            db_user = user_context;
            db_userpic = userpics_context;
            db_post = post_context;
            db_followers = followers_context;
            _appEnvironment = appEnvironment;
            db_admins = admins_context;
        }

        [Route("{login}")]
        public async Task<IActionResult> Index(string login)
        {
            if (login != null)
            {
                User userOwner = await db_user.Users
                        .Include(u => u.UserInfo)
                        .Include(u => u.UserInfo.Bguserpic)
                        .Include(u => u.UserInfo.Userpic)
                        .FirstOrDefaultAsync(u => u.Login == login);
                User user = await db_user.Users
                        .Include(u => u.UserInfo)
                        .Include(u => u.UserInfo.Bguserpic)
                        .Include(u => u.UserInfo.Userpic).FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

                Admin admin = await db_admins.Admins.FirstOrDefaultAsync(u => u.UserId == user.Id);

                Admin adminOwner = await db_admins.Admins.FirstOrDefaultAsync(u => u.UserId == userOwner.Id);

                ViewBag.Lang = user.UserInfo.Lang;

                ViewBag.Theme = user.UserInfo.Theme;

                if (admin != null)
                {
                    ViewBag.isAdmin = true;
                }
                else
                {
                    ViewBag.isAdmin = false;
                }

                if (adminOwner != null)
                {
                    ViewBag.isAdminOwner = true;
                }
                else
                {
                    ViewBag.isAdminOwner = false;
                }

                if (userOwner != null)
                {
                    if (User.Identity.Name == userOwner.Login)
                    {
                        ViewBag.owner = true;
                    }
                    else
                    {
                        ViewBag.owner = false;

                        UserFollower following = await db_followers.Following
                            .Include(u => u.User)
                            .Include(u => u.User.UserInfo)
                            .FirstOrDefaultAsync(u => u.FollowerId == user.Id && u.UserID == userOwner.Id);

                        if (following != null)
                        {
                            ViewBag.isFollow = true;
                        }
                        else
                        {
                            ViewBag.isFollow = false;
                        }
                    }

                    if (User.Identity.IsAuthenticated)
                    {
                        ViewBag.isAuth = true;
                    }
                    else
                    {
                        ViewBag.isAuth = false;
                    }

                    ViewBag.ownerLogin = userOwner.Login;

                    ViewBag.WallPosts = db_post.WallPosts
                        .Where(u => u.UserId == userOwner.Id)
                        .Where(u => u.Disabled == 0)
                        .OrderByDescending(s => s.Date)
                        .Include(u => u.User)
                        .Include(u => u.User.UserInfo)
                        .Include(u => u.User.UserInfo.Bguserpic)
                        .Include(u => u.User.UserInfo.Userpic);
                    ViewBag.Users = db_user.Users.ToListAsync();

                    ViewBag.pageOwner = userOwner;
                    ViewBag.user = user;
                }
                else
                {
                    return View();
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(IFormFile bgUserpic, IFormFile userpic, string name, string description)
        {
            User userOwner = await db_user.Users
                        .Include(u => u.UserInfo)
                        .Include(u => u.UserInfo.Bguserpic)
                        .Include(u => u.UserInfo.Userpic)
                        .FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (bgUserpic != null)
            {
                // путь к папке Files
                string path = "/user_img/bguserpic/" + bgUserpic.FileName;
                // сохраняем файл 

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await bgUserpic.CopyToAsync(fileStream);
                }

                Bguserpic newBgUserpic = new Bguserpic { Path = path };
                Bguserpic prevBgUserpic = userOwner.UserInfo.Bguserpic;
                db_userpic.Bguserpics.Add(newBgUserpic);
                db_userpic.SaveChanges();

                userOwner.UserInfo.BguserpicId = newBgUserpic.Id;
                db_user.UserInfos.Update(userOwner.UserInfo);
                db_user.SaveChanges();

                if (prevBgUserpic != null)
                {
                    if (prevBgUserpic.Id != 1)
                    {
                        db_userpic.Bguserpics.Remove(prevBgUserpic);
                        db_userpic.SaveChanges();
                    }
                }

                db_userpic.Bguserpics.Add(newBgUserpic);
            }

            if (userpic != null)
            {
                // путь к папке Files
                string path = "/user_img/userpic/" + userpic.FileName;
                // сохраняем файл 

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await bgUserpic.CopyToAsync(fileStream);
                }

                Userpic newUserpic = new Userpic { Path = path };
                Userpic prevUserpic = userOwner.UserInfo.Userpic;
                db_userpic.Userpics.Add(newUserpic);
                db_userpic.SaveChanges();

                userOwner.UserInfo.UserpicId = newUserpic.Id;
                db_user.UserInfos.Update(userOwner.UserInfo);
                db_user.SaveChanges();

                if (prevUserpic != null)
                {
                    if (prevUserpic.Id != 1)
                    {
                        db_userpic.Userpics.Remove(prevUserpic);
                        db_userpic.SaveChanges();
                    }
                }

                db_userpic.Userpics.Add(newUserpic);
            }

            UserInfo userInfo = await db_user.UserInfos.FirstOrDefaultAsync(u => u.Id == userOwner.UserInfoId);

            if (userInfo != null)
            {
                if (name != null)
                {
                    userInfo.Name = name;
                    db_user.UserInfos.Update(userInfo);
                    db_user.SaveChanges();
                }
                if (description != null)
                {
                    userInfo.Description = description;
                    db_user.UserInfos.Update(userInfo);
                    db_user.SaveChanges();
                }
            }

            return LocalRedirect("~/" + User.Identity.Name);
        }

        [HttpPost]
        public async Task<IActionResult> NewPost(string text)
        {
            User user = await db_user.Users.FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (user != null)
            {
                if (text != null)
                {
                    WallPost wallPost = new WallPost { Text = text, UserId = user.Id, Date = DateTime.Now, Likes = 0, Dislikes = 0, Comments = 0, Disabled = 0 };
                    db_post.WallPosts.Add(wallPost);
                    db_post.SaveChanges();
                };
            }
            return LocalRedirect("~/" + User.Identity.Name);
        }
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            WallPost post = await db_post.WallPosts.FirstOrDefaultAsync(u => u.Id == id);

            if (post != null)
            {
                post.Disabled = 1;
                db_post.WallPosts.Update(post);
                db_post.SaveChanges();
            }
            return LocalRedirect("~/" + User.Identity.Name);
        }
        [HttpPost]
        public async Task<IActionResult> Follow(int UserId)
        {
            User userOwner = await db_user.Users.FirstOrDefaultAsync(u => u.Id == UserId);

            User user = await db_user.Users
                        .Include(u => u.UserInfo)
                        .Include(u => u.UserInfo.Bguserpic)
                        .Include(u => u.UserInfo.Userpic)
                        .FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (userOwner != null && user != null)
            {
                UserInfo userInfo = await db_user.UserInfos.FirstOrDefaultAsync(u => u.Id == userOwner.UserInfoId);
                userInfo.Followers += 1;

                UserFollower userFollower = new UserFollower { UserID = UserId, FollowerId = user.Id };

                db_followers.Following.Add(userFollower);
                db_followers.SaveChanges();
                db_user.UserInfos.Update(userInfo);
                db_user.SaveChanges();
            }
            return LocalRedirect("~/" + userOwner.Login);
        }
        [HttpPost]
        public async Task<IActionResult> Unfollow(int UserId)
        {
            User userOwner = await db_user.Users.FirstOrDefaultAsync(u => u.Id == UserId);

            if (userOwner != null)
            {
                UserInfo userInfo = await db_user.UserInfos.FirstOrDefaultAsync(u => u.Id == userOwner.UserInfoId);
                userInfo.Followers -= 1;

                User user = await db_user.Users
                        .Include(u => u.UserInfo)
                        .Include(u => u.UserInfo.Bguserpic)
                        .Include(u => u.UserInfo.Userpic)
                        .FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

                UserFollower userFollower = await db_followers.Following
                            .Include(u => u.User)
                            .Include(u => u.User.UserInfo)
                            .FirstOrDefaultAsync(u => u.FollowerId == user.Id && u.UserID == userOwner.Id);

                db_followers.Following.Remove(userFollower);
                db_followers.SaveChanges();
                db_user.UserInfos.Update(userInfo);
                db_user.SaveChanges();
            }
            return LocalRedirect("~/" + userOwner.Login);
        }
        [HttpPost]
        public async Task<IActionResult> Remove(int followerId)
        {
            User follower = await db_user.Users.FirstOrDefaultAsync(u => u.Id == followerId);
            
            User user = await db_user.Users
                        .Include(u => u.UserInfo)
                        .Include(u => u.UserInfo.Bguserpic)
                        .Include(u => u.UserInfo.Userpic)
                        .FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (follower != null && user != null)
            {
                UserInfo userInfo = await db_user.UserInfos.FirstOrDefaultAsync(u => u.Id == user.UserInfoId);
                userInfo.Followers -= 1;

                

                UserFollower userFollower = await db_followers.Following
                            .Include(u => u.User)
                            .Include(u => u.User.UserInfo)
                            .FirstOrDefaultAsync(u => u.FollowerId == followerId && u.UserID == user.Id);

                db_followers.Following.Remove(userFollower);
                db_followers.SaveChanges();
                db_user.UserInfos.Update(userInfo);
                db_user.SaveChanges();
            }
            return LocalRedirect("~/" + user.Login);
        }
        [Route("{controller=User}/{action=Followers}/{login}")]
        public async Task<IActionResult> Followers(string login)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.isAuth = true;
            }
            else
            {
                ViewBag.isAuth = false;
            }

            User userOwner = await db_user.Users
                           .Include(u => u.UserInfo)
                           .Include(u => u.UserInfo.Bguserpic)
                           .Include(u => u.UserInfo.Userpic)
                           .FirstOrDefaultAsync(u => u.Login == login);
            User user = await db_user.Users
                    .Include(u => u.UserInfo)
                    .Include(u => u.UserInfo.Bguserpic)
                    .Include(u => u.UserInfo.Userpic).FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            ViewBag.Lang = user.UserInfo.Lang;

            ViewBag.Theme = user.UserInfo.Theme;

            Admin admin = await db_admins.Admins.FirstOrDefaultAsync(u => u.UserId == user.Id);

            Admin adminOwner = await db_admins.Admins.FirstOrDefaultAsync(u => u.UserId == userOwner.Id);

            if (admin != null)
            {
                ViewBag.isAdmin = true;
            }
            else
            {
                ViewBag.isAdmin = false;
            }

            if (user == userOwner)
            {
                ViewBag.isOwner = true;
            }
            else
            {
                ViewBag.isOwner = false;
            }

            if (adminOwner != null)
            {
                ViewBag.isAdminOwner = true;
            }
            else
            {
                ViewBag.isAdminOwner = false;
            }

            if (user != null)
            {
                ViewBag.Followers = db_followers.Following
                            .Include(u => u.User)
                            .Include(u => u.Follower)
                            .Include(u => u.User.UserInfo)
                            .Include(u => u.User.UserInfo.Bguserpic)
                            .Include(u => u.User.UserInfo.Userpic)
                            .Include(u => u.Follower.UserInfo)
                            .Include(u => u.Follower.UserInfo.Bguserpic)
                            .Include(u => u.Follower.UserInfo.Userpic)
                            .Where(u => u.UserID == userOwner.Id);
            }

            ViewBag.user = user;

            return View();
        }
         [Route("{controller=User}/{action=Followers}/{login}")]
        public async Task<IActionResult> Following(string login)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.isAuth = true;
            }
            else
            {
                ViewBag.isAuth = false;
            }

            User userOwner = await db_user.Users
                           .Include(u => u.UserInfo)
                           .Include(u => u.UserInfo.Bguserpic)
                           .Include(u => u.UserInfo.Userpic)
                           .FirstOrDefaultAsync(u => u.Login == login);
            User user = await db_user.Users
                    .Include(u => u.UserInfo)
                    .Include(u => u.UserInfo.Bguserpic)
                    .Include(u => u.UserInfo.Userpic).FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            ViewBag.Lang = user.UserInfo.Lang;

            ViewBag.Theme = user.UserInfo.Theme;

            Admin admin = await db_admins.Admins.FirstOrDefaultAsync(u => u.UserId == user.Id);

            Admin adminOwner = await db_admins.Admins.FirstOrDefaultAsync(u => u.UserId == userOwner.Id);

            if (admin != null)
            {
                ViewBag.isAdmin = true;
            }
            else
            {
                ViewBag.isAdmin = false;
            }

            if (user == userOwner)
            {
                ViewBag.isOwner = true;
            }
            else
            {
                ViewBag.isOwner = false;
            }

            if (adminOwner != null)
            {
                ViewBag.isAdminOwner = true;
            }
            else
            {
                ViewBag.isAdminOwner = false;
            }

            if (user != null)
            {
                ViewBag.Following = db_followers.Following
                            .Include(u => u.User)
                            .Include(u => u.Follower)
                            .Include(u => u.User.UserInfo)
                            .Include(u => u.User.UserInfo.Bguserpic)
                            .Include(u => u.User.UserInfo.Userpic)
                            .Include(u => u.Follower.UserInfo)
                            .Include(u => u.Follower.UserInfo.Bguserpic)
                            .Include(u => u.Follower.UserInfo.Userpic)
                            .Where(u => u.FollowerId == userOwner.Id);
            }

            ViewBag.user = user;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeLang(string url)
        {
            User user = await db_user.Users
                        .Include(u => u.UserInfo)
                        .Include(u => u.UserInfo.Bguserpic)
                        .Include(u => u.UserInfo.Userpic)
                        .FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (user != null)
            {
                UserInfo userInfo = await db_user.UserInfos.FirstOrDefaultAsync(u => u.Id == user.UserInfoId);

                string userLang = userInfo.Lang;

                if (userLang == "eng")
                {
                    userInfo.Lang = "rus";
                } else
                {
                    userInfo.Lang = "eng";
                }

                db_user.UserInfos.Update(userInfo);
                db_user.SaveChanges();
            }

            return Redirect(url);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeTheme(string url)
        {
            User user = await db_user.Users
                        .Include(u => u.UserInfo)
                        .Include(u => u.UserInfo.Bguserpic)
                        .Include(u => u.UserInfo.Userpic)
                        .FirstOrDefaultAsync(u => u.Login == User.Identity.Name);

            if (user != null)
            {
                UserInfo userInfo = await db_user.UserInfos.FirstOrDefaultAsync(u => u.Id == user.UserInfoId);

                string userTheme = userInfo.Theme;

                if (userTheme == "light")
                {
                    userInfo.Theme = "dark";
                }
                else
                {
                    userInfo.Theme = "light";
                }

                db_user.UserInfos.Update(userInfo);
                db_user.SaveChanges();
            }

            return Redirect(url);
        }
    }
}
