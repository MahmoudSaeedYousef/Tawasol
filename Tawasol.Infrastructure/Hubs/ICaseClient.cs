using System.Threading.Tasks;

namespace Tawasol.Infrastructure.Hubs
{
    public interface ICaseClient
    {
        Task ReceiveCaseUpdate(string action, object data);
    }
}
