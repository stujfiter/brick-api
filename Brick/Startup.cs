using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Brick.Data;
using Brick.Images;

namespace Brick
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
            switch (Environment.GetEnvironmentVariable("STORAGE"))
            {
                case "POSTGRESQL":
                    services.AddSingleton<PieceStore, PostgreSqlPieceStore>();
                    break;
                case "INMEMORY":
                default:
                    services.AddSingleton<PieceStore, InMemoryPieceStore>();
                    break;
            }

            services.AddSingleton<ImageProcessor, Images.ImageSharp.ImageSharpProcessor>();

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseCors(builder => {
                builder
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod();
            });

            app.UseMvc();
            
        }
    }
}
