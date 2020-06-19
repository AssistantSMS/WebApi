using ScrapMechanic.Domain.Configuration.Interface;

namespace ScrapMechanic.Domain.Configuration
{
    public class Database: IDatabase
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
        public int PageSize { get; set; }
    }
}
