using MediatR;
using Tawasol.Application.Common.Models;
using Tawasol.Application.Features.Cases.Commands.AddCaseAttachments;

namespace Tawasol.Application.Features.Donations.Commands.ConfirmDelivery
{
    public class ConfirmDeliveryCommand : IRequest<Result<bool>>
    {
        public Guid CaseItemId { get; set; }
        public Guid ConfirmedByUserId { get; set; }
        public int DeliveredQuantity { get; set; } // 👈 القطعة الناقصة هنا
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public FileModel DeliveryPhoto { get; set; }
    }
}