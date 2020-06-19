using System.Threading.Tasks;

namespace ScrapMechanic.Integration.Repository.Interface
{
    public interface IGithubRepository
    {
        Task<string> GetFileContents(string filename);
    }
}