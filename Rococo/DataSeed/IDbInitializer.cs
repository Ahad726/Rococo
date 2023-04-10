using System.Threading.Tasks;

namespace Rococo.DataSeed
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}