using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using ScrapMechanic.Domain.Contract;
using ScrapMechanic.Integration.Repository.Interface;

namespace ScrapMechanic.Integration.Repository
{
    public class StreamNewsScrapeRepository: BaseWebScrapeRepository, IStreamNewsScrapeRepository
    {
        public async Task<List<SteamNewsItem>> GetNewsItems(string appId, int limit = 100, int numberOfInitialPostElementsToScan = 10, int shortDescriptionLength = 250)
        {
            string url = $"https://store.steampowered.com/news/?appids={appId}";

            const string defaultCapsuleImage = "";
            const string defaultCoverImage = "";
            const string defaultLink = "https://scrapassistant.com";
            const string postContentSuffix = "...";

            List<SteamNewsItem> result = new List<SteamNewsItem>();
            await GetUrl(url, async doc =>
            {
                IHtmlCollection<IElement> newsSection = doc.QuerySelectorAll("#news");
                foreach (IElement newsPostElement in newsSection.Children("div[id^=\"post_\"]"))
                {
                    DateTime headlineDate = DateTime.MinValue;
                    IHtmlCollection<IElement> headlineDateElements = newsPostElement.QuerySelectorAll(".headline .date");
                    if (headlineDateElements.Length > 0)
                    {
                        string tempDate = headlineDateElements[0].Text();
                        DateTime.TryParse(tempDate, out headlineDate);
                    }

                    string title = "Unknown";
                    IHtmlCollection<IElement> titleElements = newsPostElement.QuerySelectorAll(".headline .posttitle a");
                    if (titleElements.Length > 0)
                    {
                        title = titleElements[0]?.Text() ?? "Unknown";
                    }

                    string link = defaultLink;
                    IHtmlCollection<IElement> linkElements = newsPostElement.QuerySelectorAll(".headline .posttitle a");
                    if (linkElements.Length > 0)
                    {
                        if (linkElements[0] is IHtmlAnchorElement)
                        {
                            link = (linkElements[0] as IHtmlAnchorElement)?.Href ?? defaultLink;
                        }
                    }

                    string capsuleImage = defaultCapsuleImage;
                    IHtmlCollection<IElement> capsuleImageElements = newsPostElement.QuerySelectorAll("img.capsule");
                    if (capsuleImageElements.Length > 0)
                    {
                        if (capsuleImageElements[0] is IHtmlImageElement)
                        {
                            capsuleImage = (capsuleImageElements[0] as IHtmlImageElement)?.Source ?? defaultCapsuleImage;
                            int questionMarkIndex = capsuleImage.IndexOf("?", StringComparison.Ordinal);
                            if (questionMarkIndex > 0)
                            {
                                capsuleImage = capsuleImage.Substring(0, questionMarkIndex);
                            }
                        }
                    }

                    string coverImage = defaultCapsuleImage;
                    string shortDescriptionContent = string.Empty;
                    IHtmlCollection<IElement> initialPostElements = newsPostElement.QuerySelectorAll(".body");
                    if (initialPostElements.Length > 0)
                    {
                        int numberToScan = new List<int> {numberOfInitialPostElementsToScan, initialPostElements[0].Children.Length}.Min();
                        for (int elementIndex = 0; elementIndex < numberToScan; elementIndex++)
                        {
                            if (string.IsNullOrEmpty(coverImage) && initialPostElements[0].Children[elementIndex] is IHtmlImageElement)
                            {
                                coverImage = (initialPostElements[0].Children[elementIndex] as IHtmlImageElement)?.Source ?? defaultCoverImage;
                                int questionMarkIndex = coverImage.IndexOf("?", StringComparison.Ordinal);
                                if (questionMarkIndex > 0)
                                {
                                    coverImage = coverImage.Substring(0, questionMarkIndex);
                                }
                            }
                        }
                        shortDescriptionContent = initialPostElements[0].InnerHtml.CleanHtml();

                        int shortDescripMaxLength = shortDescriptionLength - postContentSuffix.Length;
                        if (shortDescriptionContent.Length > shortDescripMaxLength)
                            shortDescriptionContent = shortDescriptionContent.Substring(0, shortDescripMaxLength);
                        shortDescriptionContent = shortDescriptionContent.TrimEnd() + postContentSuffix;
                    }

                    result.Add(new SteamNewsItem
                    {
                        Name = title,
                        Link = link,
                        Date = headlineDate,
                        SmallImage = capsuleImage,
                        Image = coverImage,
                        ShortDescription = shortDescriptionContent
                    });
                    if (result.Count >= limit) break;
                }
                await Task.FromResult(result);
            });

            return result;
        }

        //public async Task<List<NewsItem>> GetNewsPosts(int limit = 100)
        //{
        //    const string url = "https://www.nomanssky.com/news/";

        //    List<NewsItem> result = new List<NewsItem>();
        //    await GetUrl(url, async doc =>
        //    {
        //        IHtmlCollection<IElement> newsElements = doc.QuerySelectorAll("article.post");
        //        foreach (IElement newsElement in newsElements)
        //        {
        //            NewsItem logItem = new NewsItem();
        //            IHtmlCollection<IElement> linkElement = newsElement.QuerySelectorAll("a.display--block.position--absolute.position--top.position--bottom.position--left.position--right");
        //            if (linkElement.Length > 0)
        //            {
        //                IAttr hrefAttr = linkElement[0].Attributes.FirstOrDefault(a => a.Name.Equals("href"));
        //                if (hrefAttr != null && !string.IsNullOrEmpty(hrefAttr.Value))
        //                {
        //                    logItem.Link = hrefAttr.Value;
        //                }
        //            }

        //            IHtmlCollection<IElement> imgElement = newsElement.QuerySelectorAll("div.background--cover");
        //            if (imgElement.Length > 0)
        //            {
        //                IAttr styleAttr = imgElement[0].Attributes.FirstOrDefault(a => a.Name.Equals("style"));
        //                if (styleAttr != null && !string.IsNullOrEmpty(styleAttr.Value))
        //                {
        //                    logItem.Image = styleAttr.Value
        //                        .Replace("background-image: url(", string.Empty)
        //                        .Replace(");", string.Empty)
        //                        .Replace("'", string.Empty);
        //                }
        //            }

        //            IHtmlCollection<IElement> headingElement = newsElement.QuerySelectorAll("h3.post-title");
        //            if (headingElement.Length > 0)
        //            {
        //                logItem.Name = headingElement[0].Text().Trim();
        //            }

        //            IHtmlCollection<IElement> dateElement = newsElement.QuerySelectorAll(".post-meta span.date");
        //            if (dateElement.Length > 0)
        //            {
        //                logItem.Date = dateElement[0].Text();
        //            }

        //            IHtmlCollection<IElement> descriptionElement = newsElement.QuerySelectorAll("p");
        //            if (descriptionElement.Length > 0)
        //            {
        //                logItem.Description = descriptionElement[0].Text()
        //                    .Replace("            ", string.Empty)
        //                    .Replace("View Article", string.Empty)
        //                    .Trim();
        //            }

        //            result.Add(logItem);
        //            if (result.Count >= limit) break;
        //        }
        //        await Task.FromResult(result);
        //    });

        //    return result;
        //}
    }

    public static class HtmlHelper  // TODO move this to it's own file
    {
        public static string CleanHtml(this string html)
        {
            string htmlDocument = html.Replace("</li>", "\r\n");
            string noTags = Regex.Replace(htmlDocument, @"<[^>]*>", string.Empty).Trim();
            return noTags.AddSpaceAfterSpecialCharacter('!')
                .AddSpaceAfterSpecialCharacter('.')
                .AddSpaceAfterSpecialCharacter(',')
                .AddSpaceBeforeCapitals();
        }

        public static string AddSpaceAfterSpecialCharacter(this string html, char specialChar)
        {
            string pattern = @$"\{specialChar}[^\s-]";
            Regex rg = new Regex(pattern);
            MatchCollection matches = rg.Matches(html);
            foreach (Match match in matches)
            {
                string charToKeep = match.Value.Replace(specialChar.ToString(), string.Empty);
                html = html.Replace(match.Value, charToKeep);
            }
            return html;
        }

        public static string AddSpaceBeforeCapitals(this string html)
        {
            string pattern = @"[^\s-][A-Z]";
            Regex rg = new Regex(pattern);
            MatchCollection matches = rg.Matches(html);
            foreach (Match match in matches)
            {
                string newValue = match.Value[0] + ". " + match.Value[1];
                html = html.Replace(match.Value, newValue);
            }
            return html;
        }
    }
}
