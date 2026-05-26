namespace Tawasol.Application.DTOs.Cases;

public record CaseItemDto(
    Guid Id,
    string Name,
    string Description,
    string Type,
    
    // 🚀 التعديل الإستراتيجي: تحويل النوع لـ int صريح ومطابق للمعمارية الجديدة
    int TargetAmount,      // الكمية الكلية المطلوبة
    int PledgedAmount,     // الكمية المتعهد بها حالياً
    int FulfilledAmount,   // الكمية التي تم تسليمها فعلياً للمستفيد
    
    string Status
);
