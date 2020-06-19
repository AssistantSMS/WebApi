using ScrapMechanic.Domain.Configuration.Interface;

namespace ScrapMechanic.Domain.Configuration
{
    public class ApplicationInsights: IApplicationInsights
    {
        public bool Enabled { get; set; }
        public string InstrumentationKey { get; set; }
    }
}
