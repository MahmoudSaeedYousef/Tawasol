using Tawasol.Domain.Enums;
using Tawasol.Domain.Exceptions;

namespace Tawasol.Domain.Entities;

public class CaseItem
{
    public Guid Id { get; private set; }
    public Guid CaseId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public CaseItemType Type { get; private set; }
    
    // 🚀 التعديل 1: تحويل الـ TargetAmount لـ int صريح وإجباري لكل الأنواع (الكمية المطلوبة)
    public int TargetAmount { get; private set; }
    
    // 🚀 التعديل 2: إضافة حقول لمتابعة الكميات المتعهد بها والتي تم تسليمها فعلياً
    public int PledgedAmount { get; private set; }  // الكمية المحجوزة/المتعهد بها حالياً
    public int FulfilledAmount { get; private set; } // الكمية التي تم تسليمها للمستفيد فعلياً
    
    public CaseItemStatus Status { get; private set; }

    // 🚀 حقل محسوب Dynamic لمعرفة المتبقي المطلوب للتعهد
    public int RemainingAmount => TargetAmount - PledgedAmount;

    private CaseItem() { }

    // الـ Constructor المحدث: إجبار وجود كمية مستهدفة أكبر من صفر لأي عنصر
    public CaseItem(Guid caseId, string name, string description, CaseItemType type, int targetAmount=0)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Item name is required.");
        if (targetAmount <= 0) throw new DomainException("Target amount must be greater than zero.");

        Id = Guid.NewGuid();
        CaseId = caseId;
        Name = name;
        Description = description;
        Type = type;
        TargetAmount = targetAmount;
        PledgedAmount = 0;
        FulfilledAmount = 0;
        Status = CaseItemStatus.Available;
    }

    // 🚀 التعديل 3: ميثود التعهد الذكي بالكمية (Partial or Full Pledge)
    public void Pledge(int quantity)
    {
        if (Status == CaseItemStatus.Delivered)
            throw new DomainException("هذا العنصر تم تسليمه بالكامل للمستفيد بالفعل.");
            
        if (quantity <= 0) 
            throw new DomainException("يجب أن تكون كمية التعهد أكبر من الصفر.");
            
        if (quantity > RemainingAmount) 
            throw new DomainException($"الكمية المطلوبة ({quantity}) أكبر من الكمية المتبقية المتاحة ({RemainingAmount}).");

        PledgedAmount += quantity;

        // لو الكميات المتعهد بها غطت المطلوب بالكامل، نقلب حالة العنصر لـ Pledged
        if (PledgedAmount == TargetAmount)
        {
            Status = CaseItemStatus.Pledged;
        }
    }

    // 🚀 التعديل 4: إلغاء التعهد الجزئي (لو متبرع تراجع عن كمية معينة)
    public void RevertPledge(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        if (quantity > PledgedAmount) throw new DomainException("Cannot revert more than what is currently pledged.");

        PledgedAmount -= quantity;

        // طالما نزلنا عن الكمية المستهدفة، يرجع متاح للتعهد من ناس تانية
        if (PledgedAmount < TargetAmount)
        {
            Status = CaseItemStatus.Available;
        }
    }

    // 🚀 التعديل 5: تأكيد تسليم كمية معينة ميدانياً
    public void Fulfill(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero.");
        if (FulfilledAmount + quantity > PledgedAmount) 
            throw new DomainException("Cannot fulfill more than what has been pledged.");

        FulfilledAmount += quantity;
        
        // لو كل الكمية المطلوبة وصلت للمستفيد فعلياً، نغلق العنصر تماماً كـ Delivered
        if (FulfilledAmount == TargetAmount)
        {
            Status = CaseItemStatus.Delivered;
        }
    }
}
