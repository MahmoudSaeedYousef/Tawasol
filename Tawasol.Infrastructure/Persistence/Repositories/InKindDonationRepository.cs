using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;
using Tawasol.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tawasol.Domain.Entities;

namespace Tawasol.Infrastructure.Persistence.Repositories;

// 🚀 الوراثة من BaseRepository توفر الـ GetById و الـ Add و الـ Update تلقائياً
public class InKindDonationRepository(AppDbContext context) 
    : BaseRepository<InKindDonation>(context), IInKindDonationRepository
{
    public async Task<IEnumerable<InKindDonation>> GetByDonorIdAsync(Guid donorId, CancellationToken ct = default)
    {
        return await Context.InKindDonations
            .Where(d => d.DonorId == donorId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<InKindDonation>> GetByCaseItemIdAsync(Guid caseItemId, CancellationToken ct = default)
    {
        return await Context.InKindDonations
            .Where(d => d.CaseItemId == caseItemId)
            .ToListAsync(ct);
    }
}