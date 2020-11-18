using System.Security.Claims;
using DataLayer.Model.Core.User;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Parsia.Core.Account;


namespace Parsia.Controllers
{
    public class AccountController : Controller
    {

//        private readonly SignInManager<Users> _signInManager;
//
//        public AccountController(SignInManager<Users> signInManager)
//        {
//            _signInManager = signInManager;
//        }

        [Route("login")]
        public ActionResult Login()
        {
            return PartialView();
        }
        //        [Route("register")]
        //        public ActionResult Register()
        //        {
        //            return PartialView();
        //        }
        //        [Route("verify")]
        //        public ActionResult Verify(string id)
        //        {
        //            if (string.IsNullOrEmpty(id))
        //            {
        //                return View("_Error",new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "کد فعال سازی معتبر نمی باشد"));
        //            }
        //            else
        //            {
        //                var user = _unitOfWork.Users.Get(p => p.EmailCode == id).FirstOrDefault();
        //                if (user != null)
        //                {
        //                    if (user.IsDeleted != 0)
        //                    {
        //                        return View("_Error", new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "اطلاعاتی یافت نشد"));
        //                        
        //                    }
        //                    user.EmailCode = Guid.NewGuid().ToString();
        //                    user.Status = true;
        //                    _unitOfWork.Users.Update(user);
        //                    _unitOfWork.Users.Save();
        //                    return View("Login");
        //                }
        //                else
        //                {
        //                    return View("_Error", new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "کاربر گرامی لینک فعال سازی حساب کاربری شما منقضی شده است . لطفا مجددا در سایت عضو شده و یا از قسمت ارتباط با ما درخواستی مبنی بر فعال سازی حساب کاربری برای مدیر سایت ارسال نمایید"));
        //                }
        //            }
        //        }
        //
        //        [Route("active-phone")]
        //        public ActionResult ActivePhoneCode()
        //        {
        //            return PartialView();
        //        }
        //
        //        [Route("recovery")]
        //        public ActionResult RecoveryEmail()
        //        {
        //            return PartialView();
        //        }
        //        [Route("recover-phone")]
        //        public ActionResult RecoveryPhone()
        //        {
        //            return PartialView();
        //        }
        //
        //        [Route("recovery-password")]
        //        public ActionResult RecoveryPassword(string code)
        //        {
        //
        //            if (string.IsNullOrEmpty(code))
        //            {
        //                return View("_Error", new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "کد بازیابی معتبر نمی باشد"));
        //            }
        //            else
        //            {
        //                var user = _unitOfWork.Users.Get(p => p.EmailCode == code).FirstOrDefault();
        //                if (user != null)
        //                {
        //                    if (user.IsDeleted != 0)
        //                    {
        //                        return View("_Error", new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "اطلاعاتی یافت نشد"));
        //
        //                    }
        //
        //                    ViewBag.code = user.EmailCode;
        //                    return View();
        //                }
        //                else
        //                {
        //                    return View("_Error", new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "کد فعال سازی معتبر نمی باشد"));
        //                }
        //            }
        //        }
        //
        //        [Route("upgrade-site")]
        //        public ActionResult Upgrade()
        //        {
        //            return PartialView();
        //        }
        //        [Route("logout")]
        //        public ActionResult LogOut()
        //        {
        //            try
        //            {
        //                var tic = User.Identity.Name.GetTicket();
        //                if (tic != null)
        //                {
        //                    var user = _unitOfWork.Users.Get(p => p.EntityId == tic.UserId).FirstOrDefault();
        //                    if (user != null)
        //                    {
        //                        user.LastVisit = DateTime.Now;
        //                        _unitOfWork.Users.Update(user);
        //                        _unitOfWork.Users.Save();
        //                    }
        //
        //                    DynamicToken.Token.Remove(tic.Username);
        //                    DynamicRoles.Roles.Remove(tic.Username);
        //                    FormsAuthentication.SignOut();
        //                    return RedirectToAction("Index", "Home", new { Areas = "" });
        //                }
        //                else
        //                {
        //                    FormsAuthentication.SignOut();
        //                    return RedirectToAction("Index","Home",new{Areas=""});
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                return RedirectToAction("Index", "Home", new { Areas = "" });
        //            }
        //        }
//        [Route("provider/{provider}")]
//        public IActionResult GetProvider(string provider)
//        {
//            var redirectUrl = Url.RouteUrl("ExternalLogin", Request.Scheme);
//            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
//            return Challenge(properties,provider);
//        }
//
//
//        [Route("external-login",Name = "ExternalLogin")]
//        public IActionResult ExternalLogin()
//        {
//            var userEmail = User.FindFirstValue(ClaimTypes.Email);
//            using (var unitOfWork = new UnitOfWork())
//            {
//                //do login
//            }
//            return RedirectToRoute("index");
//        }



    }
}