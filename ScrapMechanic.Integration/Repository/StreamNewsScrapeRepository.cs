using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AngleSharp.Dom;
using Newtonsoft.Json;
using ScrapMechanic.Data.Helper;
using ScrapMechanic.Domain.Contract;
using ScrapMechanic.Integration.Contract;
using ScrapMechanic.Integration.Repository.Interface;

namespace ScrapMechanic.Integration.Repository
{
    public class StreamNewsScrapeRepository: BaseWebScrapeRepository, IStreamNewsScrapeRepository
    {
        public async Task<List<SteamNewsItem>> GetNewsItems(string appId, int limit = 100, int numberOfInitialPostElementsToScan = 10, int shortDescriptionLength = 250)
        {
            string url = $"https://store.steampowered.com/newshub/app/{appId}";
            string steamCommunityPublicImages = "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/clans/";

            const string defaultCoverImage = "11471984/603024a52737dc0483b07166d8ea40652c91e337.png";
            const string postContentSuffix = "...";

            List<SteamNewsItem> result = new List<SteamNewsItem>();
            await GetUrl(url, async doc =>
            {
                IElement applicationHostElement = doc.GetElementById("application_config");
                
                foreach (IAttr applicationHostAttribute in applicationHostElement.Attributes)
                {
                    if (!applicationHostAttribute.Name.Equals("data-initialEvents", StringComparison.InvariantCultureIgnoreCase)) continue;

                    string htmlEncodedString = applicationHostAttribute.Value;
                    string newsItemsJson = HttpUtility.HtmlDecode(htmlEncodedString) ?? string.Empty;

                    SteamNewsHub webObject;
                    try
                    {
                        webObject = JsonConvert.DeserializeObject<SteamNewsHub>(newsItemsJson);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    if (webObject == null) continue;

                    foreach (Event webObjectEvent in webObject.Events)
                    {
                        string coverImage = defaultCoverImage;
                        Regex steamImageRegex = new Regex(@"\[img\]\{STEAM_CLAN_IMAGE\}.+?\[\/img\]");
                        if (steamImageRegex.IsMatch(webObjectEvent.AnnouncementBody.Markdown))
                        {
                            coverImage = steamImageRegex.Match(webObjectEvent.AnnouncementBody.Markdown).Value
                                .Replace("[img]{STEAM_CLAN_IMAGE}", string.Empty)
                                .Replace("[/img]", string.Empty);
                        }
                        else
                        {
                            Regex externalImageRegex = new Regex(@"\[img\].+?\[\/img\]");
                            if (externalImageRegex.IsMatch(webObjectEvent.AnnouncementBody.Markdown))
                            {
                                coverImage = externalImageRegex.Match(webObjectEvent.AnnouncementBody.Markdown).Value
                                    .Replace("[img]{STEAM_CLAN_IMAGE}", string.Empty)
                                    .Replace("[/img]", string.Empty);
                            }
                        }

                        string descriptionInput = webObjectEvent.AnnouncementBody.Markdown.Replace("[img]{STEAM_CLAN_IMAGE}"+coverImage+"[/img]", string.Empty);
                        string taglessDescription = Regex.Replace(descriptionInput, @"\[\/*[a-z]+\]", string.Empty)
                            .Replace("\n\n\n", " ")
                            .Replace("\n\n", " ")
                            .Replace("\n", " ");

                        int shortDescripMaxLength = shortDescriptionLength - postContentSuffix.Length;
                        if (taglessDescription.Length > shortDescripMaxLength)
                            taglessDescription = taglessDescription.Substring(0, shortDescripMaxLength);
                        taglessDescription = taglessDescription.TrimEnd() + postContentSuffix;

                        string videoLink = string.Empty;
                        if (!string.IsNullOrEmpty(webObjectEvent.VideoType) && webObjectEvent.VideoType.Equals("youtube"))
                        {
                            videoLink = "https://www.youtube.com/watch?v=" + webObjectEvent.VideoPreviewId;
                        }

                        result.Add(new SteamNewsItem
                        {
                            Name = webObjectEvent.Name,
                            Link = $"{url}/view/{webObjectEvent.Id}",
                            Date = DateHelper.UnixTimeStampToDateTime(webObjectEvent.PostTime),
                            Image = $"{steamCommunityPublicImages}{coverImage}",
                            ShortDescription = taglessDescription,
                            VideoLink = videoLink,
                            UpVotes = webObjectEvent.UpVotes,
                            DownVotes = webObjectEvent.DownVotes,
                        });
                        if (result.Count >= limit) break;
                    }
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
            string pattern = $@"\{specialChar}[^\s-]";
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
