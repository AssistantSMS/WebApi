using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using ScrapMechanic.Api.Helper;
using ScrapMechanic.Data.Cache.Interface;
using ScrapMechanic.Domain.Dto.Enum;

namespace ScrapMechanic.Api.Filter
{
    public class CacheFilterAttribute : TypeFilterAttribute
    {
        public CacheFilterAttribute(CacheType cacheType, bool includeUrl = false, int numMinutes = 10) : base(typeof(CacheFilter))
        {
            Arguments = new object[] { cacheType, includeUrl, numMinutes };
        }
    }

    public class CacheFilter : IActionFilter
    {
        private readonly CacheType _cacheType;
        private readonly ICustomCache _cache;
        private readonly bool _includeUrl;
        private readonly int _numMinutes;

        public CacheFilter(CacheType cacheType, bool includeUrl, int numMinutes, ICustomCache cache)
        {
            _includeUrl = includeUrl;
            _numMinutes = numMinutes;
            _cacheType = cacheType;
            _cache = cache;
        }

        private string GetCacheKey(HttpRequest req) =>
            _includeUrl 
                ? $"{_cacheType}-{req.Path}" 
                : _cacheType.ToString();

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string cacheKey = GetCacheKey(context.HttpContext.Request);
            if (!_cache.TryGetValue(cacheKey, out ActionExecutedContext previousResponse)) return;

            context.Result = previousResponse.Result;
            context.HttpContext.Response.ApplyResponseFromCacheHeader();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                if ((context.Result as dynamic).StatusCode != 200) return;
            }
            catch (Exception)
            {
                // unused
            }

            string cacheKey = GetCacheKey(context.HttpContext.Request);
            _cache.Set(cacheKey, context, TimeSpan.FromMinutes(_numMinutes));
        }
    }
}
