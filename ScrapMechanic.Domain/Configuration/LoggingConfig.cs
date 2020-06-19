using ScrapMechanic.Domain.Configuration.Interface;

namespace ScrapMechanic.Domain.Configuration
{
    public class Logging: ILogging
    {
        public string Default { get; set; }
        public string System { get; set; }
        public string Microsoft { get; set; }
    }
}
