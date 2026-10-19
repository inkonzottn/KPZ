namespace OnlineCourses.App;

using OnlineCourses.Core.Domain;

public static class RegistrationItemParser
{
    public static Result<RegistrationItem> Parse(string courseTitle, string qtyStr, string priceStr)
    {
        if (string.IsNullOrWhiteSpace(courseTitle))
        {
            return Result<RegistrationItem>.Fail("Назва курсу не може бути порожньою.");
        }

        if (!int.TryParse(qtyStr, out var quantity) || quantity <= 0)
        {
            return Result<RegistrationItem>.Fail("Кількість — ціле число більше 0.");
        }

        if (!decimal.TryParse(priceStr, out var price) || price < 0)
        {
            return Result<RegistrationItem>.Fail("Ціна не може бути від'ємною.");
        }

        return Result<RegistrationItem>.Ok(new RegistrationItem(courseTitle, quantity, price));
    }
}