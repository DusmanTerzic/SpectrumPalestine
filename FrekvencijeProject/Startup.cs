using FrekvencijeProject.Areas.Identity.Data;
using FrekvencijeProject.Areas.Identity.Pages.Account.Manage;
using FrekvencijeProject.Controllers;
using FrekvencijeProject.Data;
using FrekvencijeProject.Models;
using FrekvencijeProject.Models.Ajax;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject
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
            services.AddDbContext<UsersDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddDbContext<AllocationDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddDbContext<ApplicationTermsDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddDbContext<AllocationTermsDBContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddDbContext<ApplicationDBContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddDbContext<StandardsDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddDbContext<ImportTempTableContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddDbContext<ImportTempInterfacesDBContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddDbContext<ImportTempRightOfUseDBContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddDbContext<Tracking_tracing_data_acqDBContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddDbContext<SRDContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("AuthDBContextConnection")));

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });
            services.AddMvc().AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix,
                opt => { opt.ResourcesPath = "Resources"; }).AddDataAnnotationsLocalization();

            services.AddScoped<HomeController>();

            services.AddTransient<FreqBandController>();
            services.AddTransient<AllocationSearchController>();
            services.AddTransient<ApplicationSearchController>();
            services.AddTransient<AdministrationPanelController>();
            services.AddTransient<DocumentController>();
            services.AddTransient<SRDController>();
            services.AddTransient<InterfacesController>();
            services.AddTransient<RightOfUseController>();
            services.AddScoped<UploadExcelController>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.Configure<RequestLocalizationOptions>(
                opt =>{
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en"),
                        new CultureInfo("ar")
                    };

                    opt.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
                    opt.SupportedCultures = supportedCultures;
                    opt.SupportedUICultures = supportedCultures;
                }
                );

            services.Configure<FormOptions>(options =>
            {
                options.KeyLengthLimit = int.MaxValue;
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
                
            });
            services.AddMvc(options =>
            {
                options.MaxModelBindingCollectionSize = 1500;
            });

           // services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthDBContext>();
           
    
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home";
                    await next();
                }
            });

            //var supportedCulteres = new[] { "en", "fr" };
            //var locallizationOptions = new RequestLocalizationOptions().
            //    SetDefaultCulture(supportedCulteres[0]).
            //    AddSupportedCultures(supportedCulteres).
            //    AddSupportedUICultures(supportedCulteres);

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService < IOptions<RequestLocalizationOptions>>().Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        
    }
}
