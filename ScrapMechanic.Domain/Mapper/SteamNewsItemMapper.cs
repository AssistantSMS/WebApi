using System.Collections.Generic;
using System.Linq;
using ScrapMechanic.Domain.Contract;
using ScrapMechanic.Domain.Dto;

namespace ScrapMechanic.Domain.Mapper
{
    public static class SteamNewsItemMapper
    {
        public static SteamNewsItemViewModel ToViewModel(this SteamNewsItem domain)
        {
            SteamNewsItemViewModel vm = new SteamNewsItemViewModel
            {
                Name = domain.Name,
                Link = domain.Link,
                Image = domain.Image,
                Date = domain.Date,
                ShortDescription = domain.ShortDescription,
                SmallImage = domain.SmallImage,
            };
            return vm;
        }

        public static List<SteamNewsItemViewModel> ToViewModel(this List<SteamNewsItem> domain) =>
            domain.Select(d => d.ToViewModel()).ToList();
    }
}
