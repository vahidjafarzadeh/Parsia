using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataLayer.Context;
using DataLayer.Model.Core.User;
using DataLayer.Token;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Parsia.Core.User;

namespace Parsia.Core.Account
{
    public class AccountService : ControllerBase
    {
        private readonly IJwtHandlers _jwtHandlers;
        private readonly IMemoryCache _memoryCache;
        private readonly UserSessionManager _userSessionManager;
        private readonly IViewRenderService _viewRenderService;

        public AccountService(IMemoryCache memoryCache, IJwtHandlers jwtHandlers, IViewRenderService viewRenderService)
        {
            _memoryCache = memoryCache;
            _jwtHandlers = jwtHandlers;
            _viewRenderService = viewRenderService;
            _userSessionManager = new UserSessionManager(memoryCache);
        }

        [HttpPost]
        [Route("service/account/login")]
        public ServiceResult<object> Login()
        {
            try
            {
                Users user;
                var userName = Request.Form["username"].ToString();
                var password = Request.Form["password"].ToString();
                var captcha = Request.Form["captcha"].ToString();
                if (string.IsNullOrEmpty(userName))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا نام کاربری را وارد نمایید");
                if (string.IsNullOrEmpty(password))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا کلمه عبور را وارد نمایید");
                if (string.IsNullOrEmpty(captcha))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا کد امنیتی را وارد نمایید");
                if (!DataLayer.Tools.Captcha.ValidateCaptchaCode(captcha, Request.HttpContext))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد امنیتی صحیح نمی باشد");
                using (var unitOfWork = new UnitOfWork())
                {
                    user = unitOfWork.Users.Get(p => p.Username == userName.ToLower().Trim()).FirstOrDefault();
                    if (user != null)
                    {
                        var pas = UserFacade.GetInstance().GetHashPassword(password);
                        if (user.Attempt >= DataLayer.Tools.SystemConfig.MaxAttemptLogin)
                        {
                            user.Attempt = Convert.ToInt16(user.Attempt + 1);
                            unitOfWork.Users.Update(user);
                            unitOfWork.Users.Save();
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                                "حساب کاربری مورد نظر مسدود می باشد");
                        }

                        if (user.Active == false)
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                                "حساب کاربری مورد نظر مسدود می باشد");
                        if (pas != user.Password)
                        {
                            user.Attempt = Convert.ToInt16(user.Attempt + 1);
                            unitOfWork.Users.Update(user);
                            unitOfWork.Users.Save();
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                                "کاربری با این مشخصات یافت نشد");
                        }
                    }
                    else
                    {
                        return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                            "کاربری با این مشخصات یافت نشد");
                    }
                }

                var userInfos = new List<UserInfo>();
                using (var context = new ParsiContext())
                {
                    var userRoles = context.UserRole.Where(p => p.UserId == user.EntityId)
                        .Include(p => p.CurrentRole)
                        .Include(p => p.CurrentOrganization)
                        .ToList();
                    userInfos.AddRange(userRoles.Select(item => new UserInfo
                    {
                        Active = true,
                        AccessKey = "",
                        Token = "",
                        FirstName = "",
                        LastName = "",
                        Password = "",
                        Username = "",
                        PersonId = -1,
                        RoleId = item.RoleId,
                        UserId = -1,
                        RoleName = item.CurrentRole.RoleName,
                        OrganizationName = item.CurrentOrganization.Name,
                        OrganizationId = item.OrganizationId,
                        Picture = ""
                    }));
                }

                if (userInfos.Count == 0)
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "کاربری با این مشخصات یافت نشد");
                return new ServiceResult<object>(userInfos, userInfos.Count);
            }
            catch (Exception e)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, e.Message);
            }
        }

        [HttpPost]
        [Route("service/account/selectUserRole")]
        public ServiceResult<object> SelectUserRole([FromBody] UserInfo userInfo)
        {
            if (userInfo.RoleId == DataLayer.Tools.SystemConfig.SystemRoleId)
            {
                var clientIp = Util.GetClientIp(Request);
                if (!DataLayer.Tools.SystemConfig.AdminValidIp.Contains(clientIp))
                    return new ServiceResult<object>(Enumerator.ErrorCode.AccessDeny,
                        $"Admin IP is invalid: {clientIp}");
            }

            var httpSession = HttpContext.Session;
            var ip = Util.GetClientIp(Request);
            if (httpSession == null || string.IsNullOrEmpty(httpSession.Id))
                return new ServiceResult<object>(Enumerator.ErrorCode.BusinessMessage,
                    "داده های ارسالی معتبر نمی باشد");
            var requestedUrl = Request.Headers["referer"].ToString();
            if (requestedUrl == null)
                return new ServiceResult<object>(Enumerator.ErrorCode.BusinessMessage, "درخواست معتبر نمی باشد");
            try
            {
                if (string.IsNullOrEmpty(userInfo.Username.Trim()))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا نام کاربری را وارد نمایید");
                if (string.IsNullOrEmpty(userInfo.Password.Trim()))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا کلمه عبور را وارد نمایید");
                var token = UserFacade.GetInstance()
                    .GetHashPassword(Util.GetTimeStamp(DateTime.Now).ToString(CultureInfo.InvariantCulture));

                using (var context = new ParsiContext())
                {
                    var userRole = context.UserRole.Where(p =>
                            p.CurrentUsers.Username == userInfo.Username.ToLower().Trim()
                            && p.CurrentUsers.Password ==
                            UserFacade.GetInstance().GetHashPassword(userInfo.Password.Trim())
                            && p.RoleId == userInfo.RoleId &&
                            p.OrganizationId == userInfo.OrganizationId)
                        .Include(p => p.CurrentRole)
                        .Include(p => p.CurrentOrganization)
                        .Include(p => p.CurrentUsers)
                        .ThenInclude(p => p.CurrentPerson)
                        .ThenInclude(p => p.CurrentFile)
                        .FirstOrDefault();
                    if (userRole != null)
                    {
                        var info = new UserInfo
                        {
                            Active = userRole.CurrentUsers.Active,
                            AccessKey = userRole.OrgAccess,
                            Token = token,
                            FirstName = userRole.CurrentUsers.FirstName,
                            LastName = userRole.CurrentUsers.LastName,
                            Password = "",
                            Username = userRole.CurrentUsers.Username,
                            PersonId = userRole.CurrentUsers.PersonId,
                            RoleId = userRole.RoleId,
                            UserId = userRole.UserId,
                            RoleName = userRole.CurrentRole.RoleName,
                            OrganizationName = userRole.CurrentOrganization.Name,
                            OrganizationId = userRole.OrganizationId,
                            Timestamp = Util.GetTimeStamp(
                                DateTime.Now.AddMinutes(
                                    Convert.ToDouble(userRole.CurrentRole.ExpireMinute.ToString()))),
                            Picture = userRole.CurrentUsers.CurrentPerson?.CurrentFile?.Path
                        };
                        info.UseCase = new Dictionary<string, HashSet<string>>();
                        var accessGroup = context.RoleAccessGroup.Where(p => p.Role == info.RoleId)
                            .Select(p => p.AccessGroup)
                            .ToList();
                        if (accessGroup.Count > 0)
                        {
                            var data = context.UseCaseActionAccessGroup.Where(p => accessGroup.Contains(p.AccessGroup))
                                .Include(p => p.CurrentUseCaseAction)
                                .ThenInclude(p => p.CurrentUseCase)
                                .Include(p => p.CurrentUseCaseAction)
                                .ThenInclude(p => p.CurrentAction).ToList();
                            foreach (var item in data)
                                if (info.UseCase.ContainsKey(item.CurrentUseCaseAction.CurrentUseCase.Clazz.ToLower()))
                                {
                                    var current =
                                        info.UseCase[item.CurrentUseCaseAction.CurrentUseCase.Clazz.ToLower()];
                                    current.Add(item.CurrentUseCaseAction.CurrentAction.ActionEnName);
                                    info.UseCase.Remove(item.CurrentUseCaseAction.CurrentUseCase.Clazz.ToLower());
                                    info.UseCase.Add(item.CurrentUseCaseAction.CurrentUseCase.Clazz.ToLower(), current);
                                }
                                else
                                {
                                    var current = new HashSet<string>();
                                    current.Add(item.CurrentUseCaseAction.CurrentAction.ActionEnName);
                                    info.UseCase.Add(item.CurrentUseCaseAction.CurrentUseCase.Clazz.ToLower(), current);
                                }
                        }

                        var offset =
                            new DateTimeOffset(
                                DateTime.Now.AddMinutes(
                                    Convert.ToDouble(userRole.CurrentRole.ExpireMinute.ToString())));
                        var option = new MemoryCacheEntryOptions().SetAbsoluteExpiration(offset)
                            .SetPriority(CacheItemPriority.High);
                        _memoryCache.Set("session_" + info.Username, info, option);

                        var claims = new List<Claim>
                        {
                            new Claim("userId", info.UserId.ToString()),
                            new Claim(ClaimTypes.Name, info.Username),
                            new Claim("username", info.Username),
                            new Claim("firstName", info.FirstName),
                            new Claim("lastName", info.LastName),
                            new Claim("roleName", info.RoleName),
                            new Claim("token", info.Token),
                            new Claim("picture", info.Picture ?? "images/users/avatar.png"),
                            new Claim("IsAdmin", userRole.CurrentUsers.IsAdmin.ToString())
                        };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        var properties = new AuthenticationProperties
                        {
                            IsPersistent = userInfo.Remember
                        };
                        HttpContext.SignInAsync(principal, properties);
                        var newUserInfo = new SimpleUserInfo
                        {
                            Token = info.Token,
                            Username = info.Username,
                            FirstName = info.FirstName,
                            LastName = info.LastName,
                            RoleName = info.RoleName,
                            OrganizationName = info.OrganizationName,
                            Timestamp = Util.GetTimeStamp(
                                DateTime.Now.AddMinutes(Convert.ToDouble(userRole.CurrentRole.ExpireMinute.ToString())))
                        };
                        var ticket = _jwtHandlers.Create(new TokenOption
                        {
                            UserInfo = newUserInfo,
                            ExpireMinutes = userRole.CurrentRole.ExpireMinute,
                            Ip = ip
                        });
                        newUserInfo.Ticket = ticket.Ticket;
                        return new ServiceResult<object>(newUserInfo, 1);
                    }

                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "کاربری با این مشخصات یافت نشد");
                }
            }
            catch (Exception e)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.BusinessMessage,
                    "امکان ورود به سایت در حال حاضر میسر نمی باشد");
            }
        }

        [HttpPost]
        [Route("service/account/register")]
        public async Task<ServiceResult<object>> Register()
        {
            try
            {
                var userName = Request.Form["username"].ToString();
                var password = Request.Form["password"].ToString();
                var confirmPassword = Request.Form["confirmPassword"].ToString();
                var law = Convert.ToBoolean(Request.Form["law"]);
                var captcha = Request.Form["captcha"].ToString();
                if (string.IsNullOrEmpty(userName))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا نام کاربری را وارد نمایید");
                if (string.IsNullOrEmpty(password))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا کلمه عبور را وارد نمایید");
                if (string.IsNullOrEmpty(confirmPassword))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا تکرار کلمه عبور را وارد نمایید");
                if (string.IsNullOrEmpty(captcha))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا کد امنیتی را وارد نمایید");
                if (!DataLayer.Tools.Captcha.ValidateCaptchaCode(captcha.Trim().ToLower(), Request.HttpContext))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد امنیتی صحیح نمی باشد");
                if (password != confirmPassword)
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "کلمه عبور با تکرار کلمه عبور مغایرت دارد");
                if (!userName.Trim().IsValidEmail() && !userName.Trim().IsValidNumber())
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "نام کاربری شما مورد تایید نمی باشد لطفا یا یک ایمیل معتبر یا یک شماره تلفن همراه 11 رقمی وارد نمایید");
                if (law)
                {
                    if (userName.Trim().IsValidEmail())
                        using (var unitOfWork = new UnitOfWork())
                        {
                            var existUser = unitOfWork.Users.Get(p => p.Username == userName.ToLower().Trim())
                                .FirstOrDefault();
                            if (existUser != null)
                                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                                    "کاربر گرامی با عرض پوزش این نام کاربری قبلا ثبت گردیده است لطفا یک نام کاربری دیگر را امتحان نمایید");

                            if (password.Length < 8)
                                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                                    "کاربر گرامی تعداد کاراکتر های مجاز برای کلمه عبور حداقل 8 رقم می باشد");

                            var person = new DataLayer.Model.Core.Person.Person
                            {
                                FirstName = "کاربر",
                                LastName = "کاربر",
                                NationalCode = "1",
                                BirthPlace = "تهران",
                                BirthDate = DateTime.Now,
                                Created = DateTime.Now,
                                Updated = DateTime.Now,
                                Code = "-",
                                CreateBy = 1,
                                UpdateBy = 1,
                                Active = true,
                                Deleted = 0
                            };
                            unitOfWork.Person.Insert(person);
                            unitOfWork.Users.Save();
                            var user = new Users
                            {
                                PersonId = person.EntityId,
                                Username = userName.Trim().ToLower(),
                                EmailCode = Guid.NewGuid().ToString(),
                                Password = UserFacade.GetInstance().GetHashPassword(password.Trim()),
                                PhoneCode = Util.RandomNumber(),
                                Attempt = 0,
                                LastVisit = DateTime.Now,
                                FirstName = "کاربر",
                                LastName = "کاربر",
                                Created = DateTime.Now,
                                Updated = DateTime.Now,
                                CreateBy = 1,
                                UpdateBy = 1,
                                Active = false,
                                IsAdmin = false,
                                Code = "-",
                                Deleted = 0
                            };
                            user.Active = false;
                            unitOfWork.Users.Insert(user);
                            unitOfWork.Users.Save();
                            var userRole = new DataLayer.Model.Core.UserRole.UserRole
                            {
                                RoleId = 2,
                                OrganizationId = 1,
                                OrgAccess = "AAA",
                                UserId = user.EntityId,
                                Created = DateTime.Now,
                                Updated = DateTime.Now,
                                CreateBy = 1,
                                UpdateBy = 1,
                                Active = true,
                                Deleted = 0
                            };
                            unitOfWork.UserRole.Insert(userRole);
                            unitOfWork.Users.Save();
                            var body = await _viewRenderService.RenderToStringAsync("Account/RegisterEmail", user);
                            SendEmail.Send(userName.Trim().ToLower(), "فعال سازی حساب کاربری", body,
                                DataLayer.Tools.SystemConfig.EmailSiteName);
                            return new ServiceResult<object>("email", 1);
                        }

                    var c = userName.ToLower().Trim().FirstOrDefault();
                    if (c != '0')
                        return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                            "شماره تلفن همراه معتبر نمی باشد ( با کاراکتر 0 شروع شود و 11 رقم باشد )");
                    if (userName.ToLower().Trim().Length != 11)
                        return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                            "شماره تلفن همراه معتبر نمی باشد ( با کاراکتر 0 شروع شود و 11 رقم باشد )");
                    using (var unitOfWork = new UnitOfWork())
                    {
                        var existUser = unitOfWork.Users.Get(p => p.Username == userName.ToLower().Trim())
                            .FirstOrDefault();
                        if (existUser != null)
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                                "کاربر گرامی با عرض پوزش این نام کاربری قبلا ثبت گردیده است لطفا یک نام کاربری دیگر را امتحان نمایید");

                        if (password.Length < 8)
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                                "کاربر گرامی تعداد کاراکتر های مجاز برای کلمه عبور حداقل 8 رقم می باشد");

                        var person = new DataLayer.Model.Core.Person.Person
                        {
                            FirstName = "کاربر",
                            LastName = "کاربر",
                            NationalCode = "1",
                            BirthPlace = "تهران",
                            BirthDate = DateTime.Now,
                            Created = DateTime.Now,
                            Updated = DateTime.Now,
                            CreateBy = 1,
                            Code = "-",
                            UpdateBy = 1,
                            Active = true,
                            Deleted = 0
                        };
                        unitOfWork.Person.Insert(person);
                        unitOfWork.Users.Save();
                        var user = new Users
                        {
                            PersonId = person.EntityId,
                            Username = userName.Trim().ToLower(),
                            EmailCode = Guid.NewGuid().ToString(),
                            Password = UserFacade.GetInstance().GetHashPassword(password.Trim()),
                            PhoneCode = Util.RandomNumber(),
                            Attempt = 0,
                            LastVisit = DateTime.Now,
                            FirstName = "کاربر",
                            LastName = "کاربر",
                            Code = "-",
                            Created = DateTime.Now,
                            Updated = DateTime.Now,
                            CreateBy = 1,
                            UpdateBy = 1,
                            Active = false,
                            IsAdmin = false,
                            Deleted = 0
                        };
                        unitOfWork.Users.Insert(user);
                        unitOfWork.Users.Save();
                        var userRole = new DataLayer.Model.Core.UserRole.UserRole
                        {
                            RoleId = 2,
                            OrganizationId = 1,
                            OrgAccess = "AAA",
                            UserId = user.EntityId,
                            Created = DateTime.Now,
                            Updated = DateTime.Now,
                            CreateBy = 1,
                            UpdateBy = 1,
                            Active = true,
                            Deleted = 0
                        };
                        unitOfWork.UserRole.Insert(userRole);
                        unitOfWork.Users.Save();
                        var sms = new Sms();
                        var number = Convert.ToInt64(userName.ToLower());
                        sms.VerifyCode(user.PhoneCode, number);
                        return new ServiceResult<object>("phone", 1);
                    }
                }

                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                    "لطفا جهت عضویت در سایت موافقت خود را با قوانین عضویت در سایت تایید نمایید");
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "AccountService.Register", null);
            }
        }

        [HttpPost]
        [Route("service/account/activeCode")]
        public ServiceResult<object> ActiveCode()
        {
            try
            {
                var phoneNumber = Request.Form["phoneNumber"].ToString();
                var code = Request.Form["code"].ToString();
                var captcha = Request.Form["captcha"].ToString();
                if (string.IsNullOrEmpty(phoneNumber))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا تلفن همراه را وارد نمایید");
                if (string.IsNullOrEmpty(code))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا کد فعال سازی را وارد نمایید");
                if (string.IsNullOrEmpty(captcha))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا کد امنیتی را وارد نمایید");
                if (!DataLayer.Tools.Captcha.ValidateCaptchaCode(captcha, Request.HttpContext))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد امنیتی صحیح نمی باشد");
                using (var unitOfWork = new UnitOfWork())
                {
                    var user = unitOfWork.Users
                        .Get(p => p.Username == phoneNumber.ToLower().Trim() && p.PhoneCode == code.Trim())
                        .FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Deleted != 0)
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                                "اطلاعاتی یافت نشد");
                        user.PhoneCode = Util.RandomNumber();
                        user.Active = true;
                        unitOfWork.Users.Update(user);
                        unitOfWork.Users.Save();
                        return new ServiceResult<object>(true, 1);
                    }

                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "اطلاعات ارسالی معتبر نمی باشد");
                }
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "AccountService.ActiveCode", null);
            }
        }

        [HttpPost]
        [Route("service/account/recovery")]
        public async Task<ServiceResult<object>> Recovery()
        {
            try
            {
                var username = Request.Form["username"].ToString();
                var captcha = Request.Form["captcha"].ToString();
                if (string.IsNullOrEmpty(username))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا نام کاربری را وارد نمایید");
                if (string.IsNullOrEmpty(captcha))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا کد امنیتی را وارد نمایید");
                if (!DataLayer.Tools.Captcha.ValidateCaptchaCode(captcha, Request.HttpContext))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد امنیتی صحیح نمی باشد");
                using (var unitOfWork = new UnitOfWork())
                {
                    var user = unitOfWork.Users.Get(p => p.Username == username.ToLower().Trim()).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Deleted != 0)
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری یافت نشد");

                        if (user.Username.IsValidNumber())
                        {
                            var sms = new Sms();
                            var number = Convert.ToInt64(username.ToLower().Trim());
                            sms.RecoveryPassword(user.EmailCode, number);
                            return new ServiceResult<object>("phone", 1);
                        }

                        var body = await _viewRenderService.RenderToStringAsync("Account/ForgetPassword", user);
                        SendEmail.Send(username.ToLower().Trim(), "بازیابی کلمه عبور", body,
                            DataLayer.Tools.SystemConfig.EmailSiteName);
                        return new ServiceResult<object>("email", 1);
                    }

                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری یافت نشد");
                }
            }
            catch (Exception e)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, e.Message);
            }
        }

        [HttpPost]
        [Route("service/account/changePassword")]
        public ServiceResult<object> ChangePassword()
        {
            try
            {
                var password = Request.Form["new-password"].ToString();
                var confirmPassword = Request.Form["confirm-password"].ToString();
                var code = Request.Form["code"].ToString();
                var captcha = Request.Form["captcha"].ToString();
                if (string.IsNullOrEmpty(password))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا کلمه عبور جدید را وارد نمایید");
                if (string.IsNullOrEmpty(confirmPassword))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا تکرار کلمه عبور جدید را وارد نمایید");
                if (password != confirmPassword)
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "کلمه عبور جدید با تکرار کلمه عبور جدید مغایرت دارد");
                if (password.Length < 8)
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "کلمه عبور جدید نمی تواند کمتر از 8 کاراکتر باشد");
                if (string.IsNullOrEmpty(captcha))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
                        "لطفا کد امنیتی را وارد نمایید");
                if (!DataLayer.Tools.Captcha.ValidateCaptchaCode(captcha, Request.HttpContext))
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد امنیتی صحیح نمی باشد");
                using (var unitOfWork = new UnitOfWork())
                {
                    var user = unitOfWork.Users.Get(p => p.EmailCode == code).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.Deleted != 0)
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری یافت نشد");

                        user.Password = UserFacade.GetInstance().GetHashPassword(password.Trim());
                        user.EmailCode = Guid.NewGuid().ToString();
                        unitOfWork.Users.Update(user);
                        unitOfWork.Users.Save();
                        return new ServiceResult<object>(true, 1);
                    }

                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری یافت نشد");
                }
            }
            catch (Exception e)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, e.Message);
            }
        }

        [HttpPost]
        [Route("service/account/logout")]
        public ServiceResult<object> LogOut(Clause clause)
        {
            try
            {
                var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
                if (userInfo != null)
                    using (var unitOfWork = new UnitOfWork())
                    {
                        var user = unitOfWork.Users.Get(p => p.Username == userInfo.Username.ToLower())
                            .FirstOrDefault();
                        if (user != null)
                        {
                            user.LastVisit = DateTime.Now;
                            unitOfWork.Users.Update(user);
                            unitOfWork.Users.Save();
                        }

                        _memoryCache.Remove("session_" + userInfo.Username);
                        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        return new ServiceResult<object>(Enumerator.ErrorCode.UserExpired, "");
                    }

                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری یافت نشد");
            }
            catch (Exception e)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, e.Message);
            }
        }
    }
}