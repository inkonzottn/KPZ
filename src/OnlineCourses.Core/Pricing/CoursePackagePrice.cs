namespace OnlineCourses.Core.Pricing;

/// <summary>Містить складові розрахунку вартості пакета курсів.</summary>
/// <param name="BaseTotal">Сума вартостей курсів до знижки.</param>
/// <param name="DiscountRate">Частка знижки, застосована до пакета.</param>
/// <param name="Discount">Грошова сума знижки, округлена до копійок.</param>
/// <param name="Payable">Підсумкова сума до сплати після знижки.</param>
public sealed record CoursePackagePrice(
    decimal BaseTotal,
    decimal DiscountRate,
    decimal Discount,
    decimal Payable);
