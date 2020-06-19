using System;

namespace ScrapMechanic.Domain.Dto
{
    public class SteamNewsItemViewModel
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string ShortDescription { get; set; }
    }
}
