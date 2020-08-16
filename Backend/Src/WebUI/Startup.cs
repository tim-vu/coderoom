using Application;
using Application.Common.Interfaces;
using Application.Common.Interfaces.EventBus;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebUI.Hubs;

namespace WebUI
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

            services.AddDataProtection()
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(Configuration.GetValue<string>("DataProtectionKeyDirectory")));


            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {
                builder
                    .WithOrigins("https://localhost", "https://*.codetwice.net")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.Configure<ForwardedHeadersOptions>(options => {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            services.AddControllers();

            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
            
            services.AddInfrastructure(Configuration);
            services.AddApplication();

            services.AddSignalR();

            services.AddTransient<IHubContext, HubContext>();
        
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IEventBus eventBus)
        {
         
            app.UseForwardedHeaders();
            app.UseCors("ApiCorsPolicy");

            app.UseExceptionHandler(options => options.UseStatusCodePages());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            eventBus.RegisterEventHandlers();

            if(env.IsDevelopment()){
                app.UseStaticFiles();
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<RoomHub>("/hubs/room");
                endpoints.MapHealthChecks("/healthcheck");
                endpoints.MapControllers();
            });

            
            if(env.IsDevelopment()){
                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "../../../Frontend";

                    if (env.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer("start");
                    }
                });
            }
        }
    }
}
