using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using DataLayer.Context;
using DataLayer.Model.Core.User;
using DataLayer.Token;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parsia.Core.User;


namespace Parsia.Core.Account
{
    public class AccountService : ControllerBase
    {
        private readonly IJwtHandlers _jwtHandlers;

        public AccountService(IJwtHandlers jwtHandlers)
        {
            _jwtHandlers = jwtHandlers;
        }
        [HttpPost, Route("service/account/Login")]
        public ServiceResult<object> Login()
        {
            try
            {
                var user = new Users();
                var userName = Request.Form["username"].ToString();
                var password = Request.Form["password"].ToString();
                var captcha = Request.Form["captcha"].ToString();
                if (string.IsNullOrEmpty(userName))
                {
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام کاربری را وارد نمایید");
                }
                if (string.IsNullOrEmpty(password))
                {
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کلمه عبور را وارد نمایید");
                }
                if (string.IsNullOrEmpty(captcha))
                {
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد امنیتی را وارد نمایید");
                }
                if (!DataLayer.Tools.Captcha.ValidateCaptchaCode(captcha, Request.HttpContext))
                {
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد امنیتی صحیح نمی باشد");
                }
                using (var unitOfWork = new UnitOfWork())
                {
                    user = unitOfWork.Users.Get(p => p.Username == userName).FirstOrDefault();
                    if (user != null)
                    {
                        var pas = UserFacade.GetInstance().GetHashPassword(password);
                        if (pas != user.Password)
                        {
                            user.Attempt = Convert.ToInt16(user.Attempt + 1);
                            unitOfWork.Users.Update(user);
                            unitOfWork.Users.Save();
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری با این مشخصات یافت نشد");
                        }
                        if (user.Attempt >= 5)
                        {
                            user.Attempt = Convert.ToInt16(user.Attempt + 1);
                            unitOfWork.Users.Update(user);
                            unitOfWork.Users.Save();
                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "حساب کاربری مورد نظر مسدود می باشد");
                        }
                        if (user.Active == false)
                        {

                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "حساب کاربری مورد نظر مسدود می باشد");
                        }
                    }
                    else
                    {
                        return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری با این مشخصات یافت نشد");
                    }
                }
                var userInfos = new List<UserInfo>();
                using (var context = new ParsiContext())
                {
                    var userRoles = context.UserRole.Where(p => p.UserId == user.EntityId)
                        .Include(p => p.CurrentRole)
                        .Include(p => p.CurrentOrganization)
                        .ToList();
                    userInfos.AddRange(userRoles.Select(item => new UserInfo()
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
                {
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری با این مشخصات یافت نشد");
                }
                return new ServiceResult<object>(userInfos, userInfos.Count);
            }
            catch (Exception e)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, e.Message);
            }
        }
        [HttpPost, Route("service/account/selectUserRole")]
        public ServiceResult<object> SelectUserRole([FromBody] UserInfo userInfo)
        {
            if (userInfo.RoleId == DataLayer.Tools.SystemConfig.SystemRoleId)
            {
                var clientIp = Util.GetClientIp(Request);
                if (!DataLayer.Tools.SystemConfig.AdminValidIp.Contains(clientIp))
                {
                    return new ServiceResult<object>(Enumerator.ErrorCode.AccessDeny, $"Admin IP is invalid: {clientIp}");
                }
            }
            var httpSession = HttpContext.Session;
            var ip = Util.GetClientIp(Request);
            if (httpSession == null || string.IsNullOrEmpty(httpSession.Id))
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.BusinessMessage, "داده های ارسالی معتبر نمی باشد");
            }
            var requestedUrl = Request.Headers["referer"].ToString();
            if (requestedUrl == null)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.BusinessMessage, "درخواست معتبر نمی باشد");
            }
            try
            {
                if (string.IsNullOrEmpty(userInfo.Username.Trim()))
                {
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام کاربری را وارد نمایید");
                }
                if (string.IsNullOrEmpty(userInfo.Password.Trim()))
                {
                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کلمه عبور را وارد نمایید");
                }
                var token = UserFacade.GetInstance().GetHashPassword(Util.GetTimeStamp(DateTime.Now).ToString(CultureInfo.InvariantCulture));

                using (var context = new ParsiContext())
                {
                    var userRole = context.UserRole.Where(p => p.CurrentUsers.Username == userInfo.Username
                                                                && p.CurrentUsers.Password == UserFacade.GetInstance().GetHashPassword(userInfo.Password.Trim())
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
                        var info = new UserInfo()
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
                            Timestamp = Util.GetTimeStamp(DateTime.Now.AddMinutes(Convert.ToDouble(userRole.CurrentRole.ExpireMinute.ToString()))),
                            Picture = userRole.CurrentUsers.CurrentPerson.CurrentFile.Path
                        };
                        var accessUserInfo = new AccessUserInfo();
                        accessUserInfo.UseCase = new Dictionary<string, HashSet<string>>();
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
                            {
                                if (accessUserInfo.UseCase.ContainsKey(item.CurrentUseCaseAction.CurrentUseCase.Clazz.ToLower()))
                                {
                                    HashSet<string> current =
                                        accessUserInfo.UseCase[item.CurrentUseCaseAction.CurrentUseCase.Clazz.ToLower()];
                                    current.Add(item.CurrentUseCaseAction.CurrentAction.ActionEnName);
                                    accessUserInfo.UseCase.Remove(item.CurrentUseCaseAction.CurrentUseCase.Clazz.ToLower());
                                    accessUserInfo.UseCase.Add(item.CurrentUseCaseAction.CurrentUseCase.Clazz.ToLower(), current);
                                }
                                else
                                {
                                    var current = new HashSet<string>();
                                    current.Add(item.CurrentUseCaseAction.CurrentAction.ActionEnName);
                                    accessUserInfo.UseCase.Add(item.CurrentUseCaseAction.CurrentUseCase.Clazz.ToLower(), current);
                                }
                            }
                        }
                        info.AccessUserInfos = accessUserInfo;
                        if (OnlineUser.UserSessionManager.ContainsKey(info.Username))
                        {
                            OnlineUser.UserSessionManager.Remove(info.Username);
                            OnlineUser.UserSessionManager.Add(info.Username, info);
                        }
                        else
                        {
                            OnlineUser.UserSessionManager.Add(info.Username, info);
                        }
                        var claims = new List<Claim>
                        {
                            new Claim("userId",info.UserId.ToString()),
                            new Claim("username",info.Username),
                            new Claim("firstName",info.FirstName),
                            new Claim("lastName",info.LastName),
                            new Claim("picture",info.Picture),
                            new Claim("IsAdmin",userRole.CurrentUsers.IsAdmin.ToString())
                        };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        var properties = new AuthenticationProperties
                        {
                            IsPersistent = userInfo.Remember
                        };
                        HttpContext.SignInAsync(principal, properties);
                        var newUserInfo = new SimpleUserInfo()
                        {
                            Username = info.Username,
                            FirstName = info.FirstName,
                            LastName = info.LastName,
                            RoleName = info.RoleName,
                            OrganizationName = info.OrganizationName,
                            Timestamp = Util.GetTimeStamp(DateTime.Now.AddMinutes(Convert.ToDouble(userRole.CurrentRole.ExpireMinute.ToString())))
                        };
                        var ticket = _jwtHandlers.Create(new TokenOption()
                        {
                            UserInfo = newUserInfo,
                            ExpireMinutes = userRole.CurrentRole.ExpireMinute,
                            Ip = ip
                        });
                        newUserInfo.Ticket = ticket.Ticket;
                        return new ServiceResult<object>(newUserInfo, 1);
                    }
                    else
                    {
                        return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری با این مشخصات یافت نشد");
                    }
                }
            }
            catch (Exception e)
            {
                return new ServiceResult<object>(Enumerator.ErrorCode.BusinessMessage, "امکان ورود به سایت در حال حاضر میسر نمی باشد");
            }
        }



        //        [HttpPost, Route("services/account/Register")]
        //        public ServiceResult<object> Register()
        //        {
        //            try
        //            {
        //                var userName = HttpContext.Current.Request.Form["username"];
        //                var password = HttpContext.Current.Request.Form["password"];
        //                var confirmPassword = HttpContext.Current.Request.Form["confirmPassword"];
        //                var law = HttpContext.Current.Request.Form["law"] != null;
        //                var captcha = HttpContext.Current.Request.Form["captcha"];
        //                var currentCaptcha = HttpContext.Current.Session["Captcha"].ToString();
        //                if (string.IsNullOrEmpty(userName))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام کاربری را وارد نمایید");
        //                }
        //                if (string.IsNullOrEmpty(password))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کلمه عبور را وارد نمایید");
        //                }
        //                if (string.IsNullOrEmpty(confirmPassword))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا تکرار کلمه عبور را وارد نمایید");
        //                }
        //                if (string.IsNullOrEmpty(captcha))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد امنیتی را وارد نمایید");
        //                }
        //                if (captcha != currentCaptcha)
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد امنیتی صحیح نمی باشد");
        //                }
        //                if (password != confirmPassword)
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کلمه عبور با تکرار کلمه عبور مغایرت دارد");
        //                }
        //                if (!userName.Trim().IsValidEmail() && !userName.Trim().IsValidNumber())
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "نام کاربری شما مورد تایید نمی باشد لطفا یا یک ایمیل معتبر یا یک شماره تلفن همراه 11 رقمی وارد نمایید");
        //                }
        //                if (law)
        //                {
        //                    if (userName.Trim().IsValidEmail())
        //                    {
        //                        var existUser = _unitOfWork.Users.Get(p => p.Username == userName && p.IsDeleted == 0)
        //                            .FirstOrDefault();
        //                        if (existUser != null)
        //                        {
        //                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
        //                                "کاربر گرامی با عرض پوزش این نام کاربری قبلا ثبت گردیده است لطفا یک نام کاربری دیگر را امتحان نمایید");
        //                        }
        //                        else if (password.Length < 8)
        //                        {
        //                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
        //                                "کاربر گرامی تعداد کاراکتر های مجاز برای کلمه عبور حداقل 8 رقم می باشد");
        //                        }
        //                        else
        //                        {
        //                            var user = new Datalayer.Model.Users()
        //                            {
        //                                Username = userName.Trim().ToLower(),
        //                                EmailCode = Guid.NewGuid().ToString(),
        //                                RegisterDate = DateTime.Now,
        //                                Status = false,
        //                                Password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5"),
        //                                PhoneCode = Tools.RandomNumber(),
        //                                IsDeleted = 0,
        //                                Attempt = 0,
        //                                LastVisit = DateTime.Now,
        //                                Created = DateTime.Now
        //                            };
        //                            var userInRole = new UserInRoles()
        //                            {
        //                                UserId = user.EntityId,
        //                                RoleId = 1,
        //                                IsDeleted = 0,
        //                                CreateBy = user.EntityId,
        //                                Created = DateTime.Now
        //                            };
        //                            var profile = new Profiles()
        //                            {
        //                                UserId = user.EntityId,
        //                                IsDeleted = 0,
        //                                CreateBy = user.EntityId,
        //                                Created = DateTime.Now,
        //                                Email = userName
        //                            };
        //
        //                            var ip = new InternetProtocols()
        //                            {
        //                                Protocol = HttpContext.Current.Request.UserHostAddress,
        //                                IsDeleted = 0,
        //                                Created = DateTime.Now,
        //                                CreateBy = user.EntityId
        //                            };
        //                            var visit = new Visits()
        //                            {
        //                                InternetProtocolId = ip.EntityId,
        //                                CreateBy = user.EntityId,
        //                                Created = DateTime.Now,
        //                                Count = 1,
        //                                IsDeleted = 0
        //                            };
        //                            _unitOfWork.Users.Insert(user);
        //                            _unitOfWork.UserInRole.Insert(userInRole);
        //                            _unitOfWork.Profile.Insert(profile);
        //                            _unitOfWork.InternetProtocol.Insert(ip);
        //                            _unitOfWork.Visit.Insert(visit);
        //                            _unitOfWork.Users.Save();
        //                            user.CreateBy = user.EntityId;
        //                            _unitOfWork.Users.Update(user);
        //                            _unitOfWork.Users.Save();
        //                            var body = PartialToStringClass.RenderPartialView("Email", "Register", user);
        //                            SendEmail.Send(userName.Trim().ToLower(), "فعال سازی حساب کاربری", body,
        //                                System.Configuration.ConfigurationManager.AppSettings["siteName"]);
        //                            return new ServiceResult<object>("email", 1);
        //
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var c = userName.ToLower().Trim().FirstOrDefault();
        //                        if (c != '0')
        //                        {
        //                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
        //                                "شماره تلفن همراه معتبر نمی باشد ( با کاراکتر 0 شروع شود و 11 رقم باشد )");
        //                        }
        //                        else if (userName.ToLower().Trim().Length != 11)
        //                        {
        //                            return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
        //                                "شماره تلفن همراه معتبر نمی باشد ( با کاراکتر 0 شروع شود و 11 رقم باشد )");
        //                        }
        //                        else
        //                        {
        //                            var existUser = _unitOfWork.Users.Get(p => p.Username == userName && p.IsDeleted == 0)
        //                                .FirstOrDefault();
        //                            if (existUser != null)
        //                            {
        //                                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
        //                                    "کاربر گرامی با عرض پوزش این نام کاربری قبلا ثبت گردیده است لطفا یک نام کاربری دیگر را امتحان نمایید");
        //                            }
        //                            else if (password.Length < 8)
        //                            {
        //                                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError,
        //                                    "کاربر گرامی تعداد کاراکتر های مجاز برای کلمه عبور حداقل 8 رقم می باشد");
        //                            }
        //                            else
        //                            {
        //                                var user = new Datalayer.Model.Users()
        //                                {
        //                                    Username = userName.Trim().ToLower(),
        //                                    EmailCode = Guid.NewGuid().ToString(),
        //                                    RegisterDate = DateTime.Now,
        //                                    Status = false,
        //                                    Password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5"),
        //                                    PhoneCode = Tools.RandomNumber(),
        //                                    IsDeleted = 0,
        //                                    Attempt = 0,
        //                                    LastVisit = DateTime.Now,
        //                                    Created = DateTime.Now
        //                                };
        //                                var userInRole = new UserInRoles()
        //                                {
        //                                    UserId = user.EntityId,
        //                                    RoleId = 1,
        //                                    IsDeleted = 0,
        //                                    CreateBy = user.EntityId,
        //                                    Created = DateTime.Now
        //                                };
        //                                var profile = new Profiles()
        //                                {
        //                                    UserId = user.EntityId,
        //                                    IsDeleted = 0,
        //                                    CreateBy = user.EntityId,
        //                                    Created = DateTime.Now,
        //                                    Phone = userName
        //                                };
        //                                var ip = new InternetProtocols()
        //                                {
        //                                    Protocol = HttpContext.Current.Request.UserHostAddress,
        //                                    IsDeleted = 0,
        //                                    Created = DateTime.Now,
        //                                    CreateBy = user.EntityId
        //                                };
        //                                var visit = new Visits()
        //                                {
        //                                    InternetProtocolId = ip.EntityId,
        //                                    CreateBy = user.EntityId,
        //                                    Created = DateTime.Now,
        //                                    Count = 1,
        //                                    IsDeleted = 0
        //                                };
        //                                _unitOfWork.Users.Insert(user);
        //                                _unitOfWork.UserInRole.Insert(userInRole);
        //                                _unitOfWork.Profile.Insert(profile);
        //                                _unitOfWork.InternetProtocol.Insert(ip);
        //                                _unitOfWork.Visit.Insert(visit);
        //                                _unitOfWork.Users.Save();
        //                                user.CreateBy = user.EntityId;
        //                                _unitOfWork.Users.Update(user);
        //                                _unitOfWork.Users.Save();
        //                                Sms sms = new Sms();
        //                                long number = Convert.ToInt64(userName.ToLower());
        //                                sms.VerifyCode(user.PhoneCode, number);
        //                                return new ServiceResult<object>("phone", 1);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا جهت عضویت در سایت موافقت خود را با قوانین عضویت در سایت تایید نمایید");
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, e.Message);
        //            }
        //        }
        //        [HttpPost, Route("services/account/ActiveCode")]
        //        public ServiceResult<object> ActiveCode()
        //        {
        //            try
        //            {
        //                var phoneNumber = HttpContext.Current.Request.Form["phoneNumber"];
        //                var code = HttpContext.Current.Request.Form["code"];
        //                var captcha = HttpContext.Current.Request.Form["captcha"];
        //                var currentCaptcha = HttpContext.Current.Session["Captcha"].ToString();
        //                if (string.IsNullOrEmpty(phoneNumber))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا تلفن همراه را وارد نمایید");
        //                }
        //                if (string.IsNullOrEmpty(code))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد فعال سازی را وارد نمایید");
        //                }
        //                if (string.IsNullOrEmpty(captcha))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد امنیتی را وارد نمایید");
        //                }
        //                if (captcha != currentCaptcha)
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد امنیتی صحیح نمی باشد");
        //                }
        //                else
        //                {
        //                    var user = _unitOfWork.Users.Get(p => p.Username == phoneNumber & p.PhoneCode == code).FirstOrDefault();
        //                    if (user != null)
        //                    {
        //                        if (user.IsDeleted != 0)
        //                        {
        //                            return new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "اطلاعاتی یافت نشد");
        //                        }
        //                        user.PhoneCode = Tools.RandomNumber();
        //                        user.Status = true;
        //                        _unitOfWork.Users.Update(user);
        //                        _unitOfWork.Users.Save();
        //                        return new ServiceResult<object>(true, 1);
        //                    }
        //                    else
        //                    {
        //                        return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "اطلاعات ارسالی معتبر نمی باشد");
        //                    }
        //
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, e.Message);
        //            }
        //        }
        //        [HttpPost, Route("services/account/Recovery")]
        //        public ServiceResult<object> Recovery()
        //        {
        //            try
        //            {
        //                var username = HttpContext.Current.Request.Form["username"];
        //                var captcha = HttpContext.Current.Request.Form["captcha"];
        //                var currentCaptcha = HttpContext.Current.Session["Captcha"].ToString();
        //                if (string.IsNullOrEmpty(username))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا نام کاربری را وارد نمایید");
        //                }
        //                if (string.IsNullOrEmpty(captcha))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد امنیتی را وارد نمایید");
        //                }
        //                if (captcha != currentCaptcha)
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد امنیتی صحیح نمی باشد");
        //                }
        //                else
        //                {
        //                    var user = _unitOfWork.Users.Get(p => p.Username == username & p.IsDeleted==0).FirstOrDefault();
        //                    if (user != null)
        //                    {
        //                        if (user.IsDeleted != 0)
        //                        {
        //                            return new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "کاربری یافت نشد");
        //                        }
        //
        //                        if (user.Username.IsValidNumber())
        //                        {
        //                            Sms sms = new Sms();
        //                            long number = Convert.ToInt64(username.ToLower());
        //                            sms.RecoveryPassword(user.EmailCode, number);
        //                            return new ServiceResult<object>("phone", 1);
        //                        }
        //                        else
        //                        {
        //                            var body = PartialToStringClass.RenderPartialView("Email", "ForgetPassword", user);
        //                            SendEmail.Send(username.ToLower().Trim(), "بازیابی کلمه عبور", body, System.Configuration.ConfigurationManager.AppSettings["siteName"]);
        //                            return new ServiceResult<object>("email", 1);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری یافت نشد");
        //                    }
        //
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, e.Message);
        //            }
        //        }
        //        [HttpPost, Route("services/account/ChangePassword")]
        //        public ServiceResult<object> ChangePassword()
        //        {
        //            try
        //            {
        //                var password = HttpContext.Current.Request.Form["new-password"];
        //                var confirmPassword = HttpContext.Current.Request.Form["confirm-password"];
        //                var code = HttpContext.Current.Request.Form["code"];
        //                var captcha = HttpContext.Current.Request.Form["captcha"];
        //                var currentCaptcha = HttpContext.Current.Session["Captcha"].ToString();
        //                if (string.IsNullOrEmpty(password))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کلمه عبور جدید را وارد نمایید");
        //                }
        //                if (string.IsNullOrEmpty(confirmPassword))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا تکرار کلمه عبور جدید را وارد نمایید");
        //                }
        //                if (password != confirmPassword)
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کلمه عبور جدید با تکرار کلمه عبور جدید مغایرت دارد");
        //                }
        //                if (password.Length < 8)
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کلمه عبور جدید نمی تواند کمتر از 8 کاراکتر باشد");
        //                }
        //                if (string.IsNullOrEmpty(captcha))
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "لطفا کد امنیتی را وارد نمایید");
        //                }
        //                if (captcha != currentCaptcha)
        //                {
        //                    return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کد امنیتی صحیح نمی باشد");
        //                }
        //                else
        //                {
        //                    var user = _unitOfWork.Users.Get(p => p.EmailCode == code & p.IsDeleted==0).FirstOrDefault();
        //                    if (user != null)
        //                    {
        //                        if (user.IsDeleted != 0)
        //                        {
        //                            return new ServiceResult<Object>(Enumerator.ErrorCode.ApplicationError, "کاربری یافت نشد");
        //                        }
        //
        //                        user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
        //                        user.EmailCode = Guid.NewGuid().ToString();
        //                        _unitOfWork.Users.Update(user);
        //                        _unitOfWork.Users.Save();
        //                        return new ServiceResult<object>(true, 1);
        //                    }
        //                    else
        //                    {
        //                        return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, "کاربری یافت نشد");
        //                    }
        //
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, e.Message);
        //            }
        //        }
        //        [HttpPost, Route("services/account/LogOut")]
        //        public ServiceResult<object> LogOut()
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
        //                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //                    return new ServiceResult<object>(new StandardResponse() { Ticket = null }, 1);
        //                }
        //                else
        //                {
        //                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //                    return new ServiceResult<object>(new StandardResponse() { Ticket = null }, 1);
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                return new ServiceResult<object>(Enumerator.ErrorCode.ApplicationError, e.Message);
        //            }
        //        }
    }
}
