using System;
using System.Linq;
using DataLayer.Context;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace Parsia.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMemoryCache _memoryCache;

        public AccountController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        [Route("login")]
        public ActionResult Login()
        {
            return PartialView();
        }
        [Route("register")]
        public ActionResult Register()
        {
            return PartialView();
        }
        [Route("active-phone")]
        public ActionResult ActivePhoneCode()
        {
            return PartialView();
        }
        [Route("verify")]
        public ActionResult Verify(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("_Error", new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد فعال سازی معتبر نمی باشد"));
            }
            else
            {
                using (var context = new ParsiContext())
                {
                    var user = context.Users.Where(p => p.EmailCode == id.Trim()).IgnoreQueryFilters().FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Deleted != 0)
                        {
                            return View("_Error", new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "اطلاعاتی یافت نشد"));

                        }
                        user.EmailCode = Guid.NewGuid().ToString();
                        user.Active = true;
                        context.Users.Update(user);
                        context.SaveChanges();
                        return Redirect("/login");
                    }
                    else
                    {
                        return View("_Error", new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربر گرامی لینک فعال سازی حساب کاربری شما منقضی شده است . لطفا مجددا در سایت عضو شده و یا از قسمت ارتباط با ما درخواستی مبنی بر فعال سازی حساب کاربری برای مدیر سایت ارسال نمایید"));
                    }
                }
            }
        }
        [Route("recovery")]
        public ActionResult RecoveryEmail()
        {
            return PartialView();
        }
        [Route("recovery-password")]
        public ActionResult RecoveryPassword(string code)
        {

            if (string.IsNullOrEmpty(code))
            {
                return View("_Error", new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد بازیابی معتبر نمی باشد"));
            }
            else
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var user = unitOfWork.Users.Get(p => p.EmailCode == code.Trim()).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Deleted != 0)
                        {
                            return View("_Error", new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "اطلاعاتی یافت نشد"));

                        }
                        ViewData["code"] = user.EmailCode;
                        return View();
                    }
                    else
                    {
                        return View("_Error", new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "کد فعال سازی معتبر نمی باشد"));
                    }
                }
            }
        }
        [Route("recover-phone")]
        public ActionResult RecoveryPhone()
        {
            return PartialView();
        }
        #region Send Email

        public IActionResult RegisterEmail()
        {
            return PartialView();
        }
        public IActionResult ForgetPassword()
        {
            return PartialView();
        }

        #endregion
        [Route("upgrade-site")]
        public ActionResult Upgrade()
        {
            return PartialView();
        }
        [Route("logout")]
        public ActionResult LogOut()
        {
            try
            {
                var username = User.Identity.Name.ToLower().Trim();
                if (!string.IsNullOrEmpty(username))
                {
                    using (var unitOfWork = new UnitOfWork())
                    {
                        var user = unitOfWork.Users.Get(p => p.Username == username).FirstOrDefault();
                        if (user != null)
                        {
                            user.LastVisit = DateTime.Now;
                            unitOfWork.Users.Update(user);
                            unitOfWork.Users.Save();
                            _memoryCache.Remove("session_" + user.Username);
                        }
                        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        return RedirectToAction("Index", "Home", new { Areas = "" });
                    }
                }
                else
                {
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return RedirectToAction("Index", "Home", new { Areas = "" });
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home", new { Areas = "" });
            }
        }
        [Route("error")]
        public ActionResult ErrorUser()
        {
            return View();
        }
    }
}