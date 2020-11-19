using System;
using System.IO;
using System.Security.Claims;
using DataLayer.Context;
using DataLayer.Token;
using DataLayer.Tools;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebMarkupMin.AspNetCore3;
using Microsoft.Extensions.Logging;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;



namespace Parsia
{
    public class Startup
    {
        private static ILogger _logger;
        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            BuildAppSettingsProvider(hostingEnvironment);
        }



        #region SystemConfig

        private void BuildAppSettingsProvider(IHostingEnvironment hostingEnvironment)
        {
            var section = Configuration.GetSection("SystemConfig");
            SystemConfig.AdminTitlePage = section["AdminTitlePage"];
            SystemConfig.Root = section["Root"];
            SystemConfig.ApiHashEncryption = section["ApiHashEncryption"];
            SystemConfig.SystemRoleId = long.Parse(section["SystemRoleId"]);
            SystemConfig.UserRoleId = long.Parse(section["UserRoleId"]);
            SystemConfig.MaxAttemptLogin = long.Parse(section["MaxAttemptLogin"]);
            SystemConfig.AdminValidIp = section["AdminValidIp"];
            SystemConfig.ApplicationUrl = section["ApplicationUrl"];
            SystemConfig.MenuCacheTimeMinute = Convert.ToDouble(section["MenuCacheTimeMinute"]);
            SystemConfig.EmailSmtpHost = section["EmailSmtpHost"];
            SystemConfig.EmailHost = section["EmailHost"];
            SystemConfig.EmailPortSmtpHost = section["EmailPortSmtpHost"];
            SystemConfig.EmailPasswordHost = section["EmailPasswordHost"];
            SystemConfig.EmailSiteName = section["EmailSiteName"];
            SystemConfig.SmsUserApiKey = section["SmsUserApiKey"];
            SystemConfig.SmsSecretKey = section["SmsSecretKey"];
            SystemConfig.TemplateIdVerificationCode = section["TemplateIdVerificationCode"];
            SystemConfig.TemplateIdRecoveryPasswordCode = section["TemplateIdRecoveryPasswordCode"];
            SystemConfig.TemplateIdRememberMeCode = section["TemplateIdRememberMeCode"];
            SystemConfig.TemplateIdUserFactorCode = section["TemplateIdUserFactorCode"];
            SystemConfig.TemplateIdAdminFactorCode = section["TemplateIdAdminFactorCode"];
            SystemConfig.TemplateIdBlockIpCode = section["TemplateIdBlockIpCode"];
        }

        #endregion


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebMarkupMin(option =>
            {
                option.AllowMinificationInDevelopmentEnvironment = true;
                option.AllowCompressionInDevelopmentEnvironment = true;
            }).AddHtmlMinification().AddHttpCompression().AddXmlMinification().AddXhtmlMinification();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDbContext<ParsiContext>(options =>
            {
                options.UseSqlServer("Data Source =.;Initial Catalog=Parsi;Integrated Security=True");
                //                options.UseSqlServer("data source=185.159.152.59;initial catalog=sensoper_ParsShoping;User Id=sensoper_ParsShoping;Password=zA9$s4d6zA9$s4d6;integrated security=False;MultipleActiveResultSets=True;");
            });
            services.AddMvc().AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; });


            services.AddCors();
            services.AddJwt(Configuration);
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            services.AddRazorPages();
            #region Max Size To Upload

            services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = int.MaxValue; });
            services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = int.MaxValue; });
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            #endregion
            #region Session for captch  
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });
            #endregion
            #region Authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.LoginPath = "/login";
                    option.LogoutPath = "/logout";
                    option.ExpireTimeSpan = TimeSpan.FromDays(1);
                }).
                AddGoogle(option =>
                {
                    option.ClientId = "53312121452-i2qomgtanfegqlh8jdb6hfuppk9etu20.apps.googleusercontent.com";
                    option.ClientSecret = "KnWOzPN9MmVKYfHUGxCS5Vq1";
                }).
                AddFacebook(options =>
                {
                    options.AppId = "1951011171673584";
                    options.AppSecret = "8ed533c721c65274a1fb21f40d29613e";
                });

            #endregion
            #region Add Memory Cache

            services.AddMemoryCache();

            #endregion
            #region Partial To String
            services.AddScoped<IViewRenderService, ViewRenderService>();
            #endregion
            #region Logging

            services.AddLogging();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            #region Logging File directory
            var path = HostingEnvironment.WebRootPath;
            loggerFactory.AddFile($"{path}\\Logs\\Log.txt");
            SystemConfig.Logger = loggerFactory.CreateLogger("ExceptionUtil");
            #endregion
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            #region session
            app.UseSession();
            #endregion
            #region AddAllHeader
            app.UseMiddleware(typeof(CustomResponseHeaderMiddleware));
            #endregion
            #region Add Client Cache

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (context) =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "public, max-age=31536000");
                    if (context.Context.Request.Path.ToString().Contains("/pages/core"))
                    {
                        var ip = Util.GetClientIp(context.Context.Request);
                        if (!SystemConfig.AdminValidIp.Contains(ip))
                        {
                            context.Context.Response.StatusCode = 403;
                            context.Context.Response.Redirect("/error");
                        }
                    }
                }
            });

            #endregion
            #region Confirm AllRequest
            app.UseMiddleware(typeof(RequestHandler));
            #endregion
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseWebMarkupMin();
            #region Authentication
            app.UseAuthentication();
            app.UseAuthorization();
            #endregion
            #region Access Admin check
            app.Use(async (context, next) =>
            {
                // Do work that doesn't write to the Response.
                if (context.Request.Path.StartsWithSegments("/Admin") || context.Request.Path.StartsWithSegments("/admin"))
                {
                    if (!context.User.Identity.IsAuthenticated)
                    {
                        context.Response.Redirect("/login");
                    }
                    else if (!Convert.ToBoolean(context.User.FindFirstValue("IsAdmin")))
                    {
                        context.Response.Redirect("/login");
                    }
                }
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });

            #endregion
            app.UseCors(builder => builder.WithOrigins(SystemConfig.ApplicationUrl.ToString()).AllowAnyHeader());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "areas",
                    "{area:exists}/{controller=Management}/{action=Index}");
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}