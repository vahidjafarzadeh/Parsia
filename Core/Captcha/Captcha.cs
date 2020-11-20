using System;
using System.IO;
using DataLayer.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Core.Captcha
{
    public class Captcha : ControllerBase
    {
        [HttpGet]
        [Route("service/captcha/get")]
        public ServiceResult<object> GetCaptchaImage()
        {
            var captchaCode = DataLayer.Tools.Captcha.GenerateCaptchaCode();
            var result = DataLayer.Tools.Captcha.GenerateCaptchaImage(100, 36, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            var base64String = Convert.ToBase64String(Util.ReadToEnd(s));
            return new ServiceResult<object>(base64String,1);
        }
       }
}
