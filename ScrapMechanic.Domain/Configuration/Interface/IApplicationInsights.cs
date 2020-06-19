namespace ScrapMechanic.Domain.Configuration.Interface
{
    public interface IApplicationInsights
    {
        bool Enabled { get; set; }
        string InstrumentationKey { get; set; }
    }
}
