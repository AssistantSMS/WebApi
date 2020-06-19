using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace ScrapMechanic.Data.Cache.Interface
{
    public interface ICustomCache : IEnumerable<KeyValuePair<object, object>>, IMemoryCache
    {
        /// <summary>
        /// Clears all cache entries.
        /// </summary>
        void Clear();
    }
}
