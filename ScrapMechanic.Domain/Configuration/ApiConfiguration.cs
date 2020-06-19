using ScrapMechanic.Domain.Configuration.Interface;

namespace ScrapMechanic.Domain.Configuration
{
    public class ApiConfiguration: IApiConfiguration
    {
        public string[] AllowedHosts { get; set; }
        public ApplicationInsights ApplicationInsights { get; set; }
        public Database Database { get; set; }
        public Jwt Jwt{ get; set; }
        public Logging Logging { get; set; }
    }
}
