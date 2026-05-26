using System;
using System.Threading.Tasks;

namespace Tawasol.Application.Interfaces.Services
{
    public interface ICaseUpdateService
    {
        Task NotifyCaseClosedAsync(Guid caseId);
        Task NotifyNewPledgeAsync(Guid caseId, Guid itemId);
        Task NotifyVillageStatsUpdatedAsync();
    }
}
