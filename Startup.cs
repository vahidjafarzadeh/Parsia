using DataLayer.Context;
using DataLayer.Token;
using DataLayer.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebMarkupMin.AspNetCore3;

namespace Parsia
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebMarkupMin(option => { option.AllowMinificationInDevelopmentEnvironment = true;
                option.AllowCompressionInDevelopmentEnvironment=true;
            }).AddHtmlMinification().AddHttpCompression().AddXmlMinification().AddXhtmlMinification();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDbContext<ParsiContext>(options =>
            {
                options.UseSqlServer("Data Source =.;Initial Catalog=Parsi;Integrated Security=True");
            });
            services.AddMvc();
            services.AddLogging();
            services.AddCors();
            services.AddJwt(Configuration);
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            services.Configure<SystemConfig>(Configuration.GetSection("SystemConfig"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");
            app.UseStaticFiles();

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