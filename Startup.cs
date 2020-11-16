using DataLayer.Context;
using DataLayer.Token;
using DataLayer.Tools;
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
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Parsia
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            BuildAppSettingsProvider(hostingEnvironment);
        }

        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        #region SystemConfig

        private void BuildAppSettingsProvider(IHostingEnvironment hostingEnvironment)
        {
            var section = Configuration.GetSection("SystemConfig");
            SystemConfig.AdminTitlePage = section["AdminTitlePage"];
            SystemConfig.Root = section["Root"];
            SystemConfig.ApiHashEncryption = section["ApiHashEncryption"];
            SystemConfig.SystemRoleId = long.Parse(section["SystemRoleId"]);
            SystemConfig.SystemAdminRoleId = long.Parse(section["SystemAdminRoleId"]);
            SystemConfig.ApplicationAdminRoleId = long.Parse(section["ApplicationAdminRoleId"]);
            SystemConfig.AdminValidIp = section["AdminValidIp"];
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
            });
            services.AddMvc().AddJsonOptions(options => { options.JsonSerializerOptions.IgnoreNullValues = true; });
            ;
            services.AddLogging();
            services.AddCors();
            services.AddJwt(Configuration);
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");
            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseWebMarkupMin();
            app.UseAuthorization();
            app.UseCors(builder =>
                builder.WithOrigins("http://localhost:52318/")
                    .AllowAnyHeader()
            );
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