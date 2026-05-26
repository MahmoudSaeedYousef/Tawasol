using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Tawasol.Application.Features.Cases.Queries.GetVillageStats;
using Tawasol.Application.Interfaces.Services;
using Tawasol.Infrastructure.Hubs;

namespace Tawasol.Infrastructure.Services
{
    public class CaseUpdateService : ICaseUpdateService
    {
        private readonly IHubContext<CaseHub, ICaseClient> _hubContext;
        private readonly IMediator _mediator;

        public CaseUpdateService(IHubContext<CaseHub, ICaseClient> hubContext, IMediator mediator)
        {
            _hubContext = hubContext;
            _mediator = mediator;
        }

        public async Task NotifyCaseClosedAsync(Guid caseId)
        {
            await _hubContext.Clients.All.ReceiveCaseUpdate("CaseClosed", new { CaseId = caseId, Status = "Closed" });
        }

        public async Task NotifyNewPledgeAsync(Guid caseId, Guid itemId)
        {
            await _hubContext.Clients.All.ReceiveCaseUpdate("NewPledge", new { CaseId = caseId, ItemId = itemId, Status = "Pledged" });
        }

        public async Task NotifyVillageStatsUpdatedAsync()
        {
            var stats = await _mediator.Send(new GetVillageStatsQuery());
            await _hubContext.Clients.All.ReceiveCaseUpdate("VillageStatsUpdated", stats);
        }
    }
}
