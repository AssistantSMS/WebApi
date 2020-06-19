using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ScrapMechanic.Data.Filter;
using ScrapMechanic.Domain.Configuration.Interface;
using ScrapMechanic.Domain.Constants;

namespace ScrapMechanic.Data.Helper
{
    public static class ThirdPartySetupHelper
    {
        public static IServiceCollection RegisterThirdPartyServicesForApi(this IServiceCollection services, IApiConfiguration config)
        {
            //services.SetUpEntityFramework(config);
            //services.SetUpJwt(config);
            services.SetUpSwagger();
            return services;
        }
        
        //private static void SetUpEntityFramework(this IServiceCollection services, IApiConfiguration config)
        //{
        //    services.AddDbContext<NmsAssistantContext>(options => options
        //        .UseLazyLoadingProxies()
        //        .UseSqlServer(config.Database.ConnectionString, dbOptions => dbOptions.MigrationsAssembly("NMS.Assistant.Api"))
        //    );
        //}
        
        //private static void SetUpJwt(this IServiceCollection services, IApiConfiguration config)
        //{

        //    byte[] key = Encoding.ASCII.GetBytes(config.Jwt.Secret);
        //    services.AddAuthentication(x =>
        //        {
        //            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //        })
        //        .AddJwtBearer(options =>
        //        {
        //            //x.RequireHttpsMetadata = false;
        //            options.SaveToken = true;
        //            options.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                ValidateIssuerSigningKey = true,
        //                IssuerSigningKey = new SymmetricSecurityKey(key),
        //                ValidateIssuer = false,
        //                ValidateAudience = false,
        //                ValidateLifetime = true,
        //                RequireExpirationTime = true,
        //                ClockSkew = TimeSpan.FromSeconds(config.Jwt.ClockSkewInSeconds)
        //            };
        //        });
        //}

        private static void SetUpSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(ApiAccess.Public, SwaggerHelper.CreateInfoForApiVersion(ApiAccess.Public));
                //c.SwaggerDoc(ApiAccess.Auth, SwaggerHelper.CreateInfoForApiVersion(ApiAccess.Auth));
                c.SwaggerDoc(ApiAccess.All, SwaggerHelper.CreateInfoForApiVersion(ApiAccess.All));
                c.DocInclusionPredicate(SwaggerHelper.DocInclusionPredicate);

                c.AddSecurityDefinition(ApiAuthScheme.Basic, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Description = "basic authentication for API",
                    In = ParameterLocation.Header,
                    Scheme = ApiAuthScheme.Basic
                });
                c.AddSecurityDefinition(ApiAuthScheme.JwtBearer, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                                  "Enter 'Bearer' [space] and then your token in the text input below." +
                                  "\r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = ApiAuthScheme.JwtBearer
                });
                c.OperationFilter<AuthorizeCheckOperationFilter>();

                // Set the comments path for the Swagger JSON and UI.
                const string xmlFile = "ScrapMechanic.Api.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
