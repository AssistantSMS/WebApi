namespace ScrapMechanic.Domain.Configuration.Interface
{
    public interface ILogging
    {
        string Default { get; set; }
        string System { get; set; }
        string Microsoft { get; set; }
    }
}
