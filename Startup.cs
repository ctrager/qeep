using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog;
using Microsoft.AspNetCore.HttpOverrides;

namespace keep
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            kp_util.log("Startup");
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            kp_util.log("ConfigureServices");

            // services.AddDistributedMemoryCache();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                // options.CheckConsentNeeded = context => false; // Default is true, make it false
                //options.CheckConsentNeeded = false;
                // So that if we click a keep link in email, it works
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddHttpContextAccessor();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            services.AddRazorPages();

            // if nginx is not on same machine
            // services.Configure<ForwardedHeadersOptions>(options =>
            // {
            //     options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
            // });


            services.AddCors(options =>
                {
                    options.AddPolicy("CoreyPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                    );
                });


            // services.Configure<ApiBehaviorOptions>(options =>
            // {
            //     options.SuppressModelStateInvalidFilter = true;

            // });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            kp_util.log("Configure");

            app.UseCors("CoreyPolicy");

            // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-5.0
            // for nginx
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //if (env.IsDevelopment())
            //{
            if (kp_config.get(kp_config.UseDeveloperExceptionPage) == 1)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();

            // for redirecting https to http - commented out because nginx is doing this for us and it's annoying in dev
            // app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseSerilogRequestLogging();

            // corey added a step in the pipeline
            app.Use(async (context, next) =>
            {

                //kp_util.log("Startup.cs URL: " + context.Request.GetDisplayUrl());
                await next.Invoke();

            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

        }
    }
}
