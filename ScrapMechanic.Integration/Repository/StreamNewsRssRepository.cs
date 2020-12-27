using System.Collections.Generic;
using System.Threading.Tasks;
using SystemOut.RssParser.Rss;
using ScrapMechanic.Domain.Contract;
using ScrapMechanic.Domain.Helper;
using ScrapMechanic.Integration.Repository.Interface;

namespace ScrapMechanic.Integration.Repository
{
    public class StreamNewsRssRepository : BaseExternalApiRepository, IStreamNewsRepository
    {
        public Task<List<SteamNewsItem>> GetNewsItems(string appId, int limit = 100, int numberOfInitialPostElementsToScan = 10, int shortDescriptionLength = 250)
        {
            string url = $"https://store.steampowered.com/feeds/news/app/{appId}";
            const string steamCommunityPublicImage = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/clans/11471984/603024a52737dc0483b07166d8ea40652c91e337.png";
            const string postContentSuffix = "...";

            List<SteamNewsItem> result = new List<SteamNewsItem>();
            
            BaseRssFeed<BaseRssChannel<SteamNewsRssItem>> feed = RssDeserializer.GetFeed<BaseRssFeed<BaseRssChannel<SteamNewsRssItem>>>(url);
            foreach (BaseRssChannel<SteamNewsRssItem> channel in feed.RssChannels)
            {
                foreach (SteamNewsRssItem item in channel.RssItems)
                {
                    string cleanHtml = item.Description.CleanHtml();
                    int shortDescripMaxLength = shortDescriptionLength - postContentSuffix.Length;
                    string finalDescription = (cleanHtml.Length > shortDescripMaxLength)
                        ? cleanHtml.Substring(0, shortDescripMaxLength)
                        : cleanHtml.TrimEnd();

                    result.Add(new SteamNewsItem
                    {
                        Name = item.Title,
                        ShortDescription = finalDescription + postContentSuffix,
                        Image = item?.Image?.Url ?? steamCommunityPublicImage,
                        Link = item.Link,
                        Date = item.Date,
                        DownVotes = 0,
                        UpVotes = 0,
                    });
                }
            }

            return Task.FromResult(result);
        }
    }
}
