using System;
using Backend.Database;
using Backend.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend
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
            services.AddControllers();
            
            //Make sure we have the database configuration and a single instance
            //of DatabaseBootstrap that can be injected into our controllers.
            var config = Configuration.GetSection("Database");
            services.AddSingleton(new DatabaseConfig 
                { 
                    Name = config.GetValue<string>("Name"), 
                    StructureFile = config.GetValue<string>("StructureFile") 
                });

            services.AddSingleton<IDatabaseService, DatabaseService>();
            
            //To add a repository for depenedency injection
            //create a class that implements IGameRepository
            services.AddSingleton<IGameRepository, GameRepository>();
            
            //Add service to save and get image. 
            //services.AddSingleton<IImageRepository, ImageRepository>();
            
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Speldatabasen");
            });

            if(env.IsProduction()) 
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            serviceProvider.GetService<IDatabaseService>().Setup();
        }
    }
}
