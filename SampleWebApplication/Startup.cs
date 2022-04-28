using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OgcApi.Net;
using OgcApi.Net.MbTiles;
using OgcApi.Net.SqlServer;
using SampleWebApplication.Utils;

namespace SampleWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOgcApiSqlServerProvider();
            services.AddOgcApiMbTilesProvider();
            services.AddOgcApi("ogcapi.json");
            services.AddControllers().AddOgcApiControllers();

            services.AddCors(c => c.AddPolicy(name: "OgcApi", options =>
            {
                options.AllowAnyMethod().AllowAnyHeader();
            }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            DatabaseUtils.RecreateDatabase();

            app.UseSwaggerUI(swaggerOptions =>
            {
                swaggerOptions.RoutePrefix = "api";
                swaggerOptions.SwaggerEndpoint("ogc/swagger.json", "OGC API");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("OgcApi");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
