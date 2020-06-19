namespace ScrapMechanic.Domain.Configuration.Interface
{
    public interface IApiConfiguration
    {
        string[] AllowedHosts { get; set; }
        ApplicationInsights ApplicationInsights { get; set; }
        Database Database { get; set; }
        Jwt Jwt { get; set; }
        Logging Logging { get; set; }
    }
}
