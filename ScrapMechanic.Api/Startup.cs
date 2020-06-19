using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScrapMechanic.Api.Helper;
using ScrapMechanic.Domain.Configuration;
using ScrapMechanic.Domain.Configuration.Interface;
using ScrapMechanic.Data.Helper;
using ScrapMechanic.Domain.Constants;

namespace ScrapMechanic.Api
{
    public class Startup
    {
        public IApiConfiguration ApiConfiguration { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            ApiConfiguration = Configuration.Get<ApiConfiguration>();

            services.RegisterCommonServices(ApiConfiguration);
            services.RegisterThirdPartyServicesForApi(ApiConfiguration);

            services.AddCors();
            services.AddRouting();
            services.AddMvc().AddNewtonsoftJson()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDatabaseErrorPage();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder =>
                builder.WithOrigins(ApiConfiguration.AllowedHosts.ToArray())
                    .WithMethods(ApiCorsSettings.AllowedMethods)
                    .WithHeaders(ApiCorsSettings.ExposedHeaders)
                    .WithExposedHeaders(ApiCorsSettings.ExposedHeaders)
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(30))
                    .AllowCredentials()
            );
            //db.Database.Migrate();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{ApiAccess.Public}/swagger.json", "AssistantSMS API - Public");
                //c.SwaggerEndpoint($"/swagger/{ApiAccess.Auth}/swagger.json", "AssistantSMS API - Authenticated");
                c.SwaggerEndpoint($"/swagger/{ApiAccess.All}/swagger.json", "AssistantSMS API - All");
                //c.InjectStylesheet("/assets/css/customSwagger.css");
                c.DocumentTitle = "AssistantSMS API Documentation";
                c.RoutePrefix = string.Empty;
                c.DisplayRequestDuration();
            });

            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() => Task.WhenAll(
                    context.Response.ApplyAuthorHeader(),
                    context.Response.ApplyDefaultCacheHeader(),
                    context.Response.ApplyDefaultResponseFromCacheHeader()
                ));
                await next();
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
