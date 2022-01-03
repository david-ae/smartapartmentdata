using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartApartmentData.Domain.JsonReader;
using SmartApartmentData.Infrastructure.Services;
using SmartApartmentData.Infrastructure.Settings.Models;

namespace SmartApartmentData.API
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
            var config = new AWSSettings();
            config.AccessKey = Configuration["AWS:AccessKey"];
            config.SecretKey = Configuration["AWS:SecretKey"];
            config.Region = Configuration["AWS:Region"];
            config.ElasticUrl = Configuration["AWS:ElasticUrl"];

            services.AddSingleton<AWSSettings>(config);
            services.AddTransient<IInfrastructureService, InfrastructureService>();

            services.AddTransient<IJsonReader, JsonReader>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
