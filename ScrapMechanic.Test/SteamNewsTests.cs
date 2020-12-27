using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScrapMechanic.Domain.Contract;
using ScrapMechanic.Integration.Repository;
using ScrapMechanic.Integration.Repository.Interface;

namespace ScrapMechanic.Test
{
    [TestClass]
    public class SteamNewsTests
    {
        [TestMethod]
        public async Task GetNewsFromHtmlScrape()
        {
            IStreamNewsRepository repo = new StreamNewsScrapeRepository();
            List<SteamNewsItem> newsItems = await repo.GetNewsItems("387990");

            Assert.IsTrue(newsItems.Count > 0);
        }

        [TestMethod]
        public async Task GetNewsFromRss()
        {
            IStreamNewsRepository repo = new StreamNewsRssRepository();
            List<SteamNewsItem> newsItems = await repo.GetNewsItems("387990");

            Assert.IsTrue(newsItems.Count > 0);
        }

        [TestMethod]
        public async Task GetNewsCombined()
        {
            IStreamNewsRepository repo = new StreamNewsRssRepository();
            List<SteamNewsItem> newsItems = await repo.GetNewsItems("387990");
        }
    }
}
