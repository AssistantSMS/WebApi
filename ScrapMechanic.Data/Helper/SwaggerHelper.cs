﻿using System;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using ScrapMechanic.Domain.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ScrapMechanic.Data.Helper
{
    public static class SwaggerHelper
    {
        //public static List<ApiVersionDefinition> DefinedVersions => new List<ApiVersionDefinition>
        //{
        //    new ApiVersionDefinition
        //    {
        //        Version = new ApiVersion(1, 0),
        //        DisplayName = "Initial Release"
        //    },
        //    new ApiVersionDefinition
        //    {
        //        Version = new ApiVersion(1, 1, "Alpha"),
        //        DisplayName = "V1.1 - Alpha"
        //    }
        //};

        /// <summary>
        /// Ask Kurt, complicated crap to allow versioning of the api 
        /// </summary>
        /// <param name="version"></param>
        /// <param name="apiDescription"></param>
        /// <returns></returns>
        public static bool DocInclusionPredicate(string version, ApiDescription apiDescription)
        {
            if (version.Equals(ApiAccess.All)) return true;

            bool isAuthReq = apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) && methodInfo.GetAttributes<AuthorizeAttribute>().Any();

            if (version.Equals(ApiAccess.Public))
            {
                dynamic controllerName = (apiDescription.ActionDescriptor as dynamic)?.ControllerName ?? string.Empty;
                if (string.IsNullOrEmpty(controllerName)) return false;
                if (!ApiAccess.PublicControllers.Contains(controllerName)) return false;
                return !isAuthReq;
            }
            
            if (version.Equals(ApiAccess.Public)) return !isAuthReq;
            //if (version.Equals(ApiAccess.Auth)) return isAuthReq;

            return false;
        }
        //apiDescription.GroupName.ToLower().Contains(version.ToLower());

        /// <summary>
        /// Ask Kurt, I abstracted common declarations
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static OpenApiInfo CreateInfoForApiVersion(string version)
        {
            string description = "API Documentation of the Assistant for Scrap Mechanic API";
            if (version.Equals(ApiAccess.Public)) description = "Easy to use endpoints, these are Public endpoints that do not need a JWT token to work. These endpoints should never return 401 or 403 error codes.";
            //if (version.Equals(ApiAccess.Auth)) description = "Authenticated endpoints that do require a valid JWT token (passed in the header 'Authorization': 'Bearer xxxxx') to work. \n\rYou can get the JWT token by logging into the /Auth controller using Basic Authentication (passed in the header 'Authorization': 'Basic xxxxx='), the token is returned in the header.";
            if (version.Equals(ApiAccess.All)) description = "All endpoints, those that are Public and those that require you to be logged in.";

            OpenApiInfo info = new OpenApiInfo
            {
                Version = version,
                Title = "Assistant for Scrap Mechanic API",
                Description = description,
                Contact = new OpenApiContact
                {
                    Name = "AssistantSMS",
                    Email = "scrap@kurtlourens.com",
                    Url = new Uri("https://scrapassistant.com")
                }
            };
            return info;
        }
    }
}
