using System.Collections.Generic;

namespace Tawasol.Application.Features.Cases.Queries.GetVillageStats
{
    public class VillageStatsResponseDto
    {
        public int TotalDeliveredItems { get; set; }
        public int TotalClosedCases { get; set; }
        public decimal TotalDonationAmount { get; set; }
        // public Dictionary<string, int> DeliveredItemsByType { get; set; } = new();
    }
}
