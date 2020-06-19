using System;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace ScrapMechanic.Integration.Repository
{
    public class BaseWebScrapeRepository
    {
        protected async Task GetUrl(string url, Func<IDocument, Task> htmlAction)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();
            IBrowsingContext context = BrowsingContext.New(config);
            using IDocument document = await context.OpenAsync(url);
            await htmlAction(document);
        }
    }
}
