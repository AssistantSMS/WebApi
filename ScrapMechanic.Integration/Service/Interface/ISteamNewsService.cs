using System.Collections.Generic;
using System.Threading.Tasks;
using ScrapMechanic.Domain.Contract;

namespace ScrapMechanic.Integration.Service.Interface
{
    public interface ISteamNewsService
    {
        Task<List<SteamNewsItem>> GetNewsItems(string appId, int limit = 100, int numberOfInitialPostElementsToScan = 10, int shortDescriptionLength = 250);
    }
}
