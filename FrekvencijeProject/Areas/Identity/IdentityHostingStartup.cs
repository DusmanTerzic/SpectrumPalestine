using System;
using FrekvencijeProject.Areas.Identity.Data;
using FrekvencijeProject.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(FrekvencijeProject.Areas.Identity.IdentityHostingStartup))]
namespace FrekvencijeProject.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AuthDBContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AuthDBContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddEntityFrameworkStores<AuthDBContext>();
            });
        }
    }
}