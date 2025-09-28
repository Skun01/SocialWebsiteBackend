using System.Threading.Tasks;

namespace SocialWebsite.Interfaces.Services;

public interface IDatabaseSeeder
{
    Task SeedAsync();
}
