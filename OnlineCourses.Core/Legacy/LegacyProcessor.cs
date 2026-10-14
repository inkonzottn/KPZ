namespace OnlineCourses.Core.Legacy;

using OnlineCourses.Core.Documents;
using static OnlineCourses.Core.Documents.DocumentTotalCalculator;

public static class LegacyProcessor
{
    public static decimal Process(
    int docId,
    string clientName,
    string clientEmail,
    bool isRegular,
    IReadOnlyList<RegistrationItem> lines,
    DateOnly createdAt,
    string couponCode,
    DocumentStatus status,
    bool printToConsole,
    decimal deliveryPrice,
    out DocumentStatus newStatus)
    {
        DocumentGuards.EnsureHeaderValid(
        docId, clientName, clientEmail);
        decimal total = SubtotalOf(lines);
        total -= ClientDiscountOf(total, isRegular);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, couponCode);
        total -= WeekendDiscountOf(total, createdAt);
        total += DeliveryFeeOf(total, deliveryPrice);
        total = status == DocumentStatus.Cancelled
        ? 0m
        : Math.Round(Math.Max(total, 0m), 2);
        newStatus = NextStatusOf(status, total);
        if (printToConsole)
        {
            Console.WriteLine("Документ No " + docId);
            Console.WriteLine("Клієнт: " + clientName);
            foreach (RegistrationItem line in lines)
            {
                Console.WriteLine(line.Sku + " x " +
                line.Quantity);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }
}
