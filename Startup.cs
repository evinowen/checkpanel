using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using checkpanel.Data;

namespace checkpanel
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
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(120);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews();
            services.AddDbContext<SqlServerContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("default"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SqlServerContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            context.Database.Migrate();

            RewriteOptions options = new RewriteOptions()
                .AddRedirect(@"^$", "/List");

            app.UseRewriter(options);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseWhen(
                context => !context.Request.Path.StartsWithSegments("/Login"),
                app => {
                    app.Use(async (context, next) =>
                    {
                        var state = context.Session.GetString("State");

                        bool redirect = true;

                        if (state == "Authenticated")
                        {
                            redirect = false;
                        }

                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            if (context.Request.Headers.ContainsKey("Authorization"))
                            {
                                string header = context.Request.Headers["Authorization"];
                                string token = header.Substring("Bearer ".Length).Trim();

                                if (CheckAuthorizationToken(token))
                                {
                                    redirect = false;
                                }
                            }
                        }

                        if (redirect) {
                            context.Response.Redirect("/Login");
                            return;
                        }

                        await next.Invoke();
                    });
                }
            );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private bool CheckAuthorizationToken(string token)
        {
            byte[] hash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Configuration["API_KEY"]));

            StringBuilder builder = new StringBuilder();
            foreach (byte b in hash)
            {
                builder.Append(b.ToString("X2"));
            }

            return token == builder.ToString();
        }
    }
}
