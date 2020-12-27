using ScrapMechanic.Domain.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrapMechanic.Integration.Repository;
using ScrapMechanic.Integration.Repository.Interface;
using ScrapMechanic.Integration.Service.Interface;

namespace ScrapMechanic.Integration.Service
{
    public class SteamNewsService: ISteamNewsService
    {
        public async Task<List<SteamNewsItem>> GetNewsItems(string appId, int limit = 100, int numberOfInitialPostElementsToScan = 10, int shortDescriptionLength = 250)
        {
            IStreamNewsRepository rssRepo = new StreamNewsRssRepository();
            IStreamNewsRepository scrapeRepo = new StreamNewsScrapeRepository();

            Task<List<SteamNewsItem>> rssTask = rssRepo.GetNewsItems(appId, limit, numberOfInitialPostElementsToScan, shortDescriptionLength);
            Task<List<SteamNewsItem>> scrapeTask = scrapeRepo.GetNewsItems(appId, limit, numberOfInitialPostElementsToScan, shortDescriptionLength);

            List<SteamNewsItem> rssItems = await rssTask;
            List<SteamNewsItem> scrapeItems = await scrapeTask;

            List<SteamNewsItem> results = new List<SteamNewsItem>();
            foreach (SteamNewsItem steamNewsItem in rssItems)
            {
                SteamNewsItem mergedItem = steamNewsItem;
                SteamNewsItem foundItem = scrapeItems.FirstOrDefault(scr => scr.Link.Equals(steamNewsItem.Link));
                if (foundItem != null)
                {
                    mergedItem.UpVotes = foundItem.UpVotes;
                    mergedItem.DownVotes = foundItem.DownVotes;
                    mergedItem.VideoLink = foundItem.VideoLink;
                    mergedItem.CommentCount = foundItem.CommentCount;
                }
                results.Add(mergedItem);
            }

            return results;
        }
    }
}
