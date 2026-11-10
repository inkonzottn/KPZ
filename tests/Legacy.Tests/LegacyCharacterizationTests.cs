using System.Globalization;
using Variant.Legacy;
using Xunit;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        // формат чисел і регістр літер залежать від культури середовища,
        // тому фіксуємо її явно
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    private static List<RegistrationItem> TwoLines() => new()
    {
        new RegistrationItem { Code = "A-1", Qty = 2, Price = 150m },
        new RegistrationItem { Code = "B-2", Qty = 1, Price = 400m }
    };

    private static List<RegistrationItem> OneLine(decimal price) => new()
    {
        new RegistrationItem { Code = "X-1", Qty = 1, Price = price }
    };

    private const string Ok = "a@b.c";
    private static readonly DateTime Created = new(2026, 3, 1);

    [Theory]
    [InlineData(null, "new", "a@b.c", "ERR: null")]
    [InlineData("empty", "new", "a@b.c", "ERR: empty")]
    [InlineData("two", "draft", "a@b.c", "ERR: state")]
    [InlineData("two", "new", "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfItems, string state, string mail, string expected)
    {
        var items = kindOfItems switch
        {
            "two" => TwoLines(),
            "empty" => new List<RegistrationItem>(),
            _ => null
        };
        var sut = new LegacyRegistrationProcessor();

        var actual = sut.Handle(1001, 7, "Іван", mail, "regular", 0,
            items, state, "UAH", Created, false);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipOrder_ReturnsExactReport()
    {
        var sut = new LegacyRegistrationProcessor();

        var report = sut.Handle(1001, 7, "Іван", Ok, "vip", 3,
            TwoLines(), "new", "UAH", Created, false);

        var expected =
            "Документ #1001\n" +
            "Клієнт: Іван\n" +
            "A-1 x2 = 300.00 UAH\n" +
            "B-2 x1 = 400.00 UAH\n" +
            "Знижка: 105.00 UAH\n" +
            "Доставка: 60.00 UAH\n" +
            "Разом: 655.00 UAH\n";
        Assert.Equal(expected, report);
    }

    [Theory]
    [InlineData("regular", 0, 760)]
    [InlineData("vip", 0, 655)]
    [InlineData("staff", 0, 550)]
    [InlineData("regular", 11, 725)]
    public void Preview_ByKind_ReturnsTotal(string kind, int done, decimal expected)
    {
        var sut = new LegacyRegistrationProcessor();

        var total = sut.Preview(kind, done, TwoLines());

        Assert.Equal(expected, total);
    }

    [Theory]
    [InlineData("vip", 0, 5000, 4500)]
    [InlineData("staff", 0, 2000, 1500)]
    [InlineData("regular", 11, 20000, 19500)]
    public void Preview_DiscountIsCappedAt500(string kind, int done,
        decimal price, decimal expected)
    {
        var sut = new LegacyRegistrationProcessor();

        var total = sut.Preview(kind, done, OneLine(price));

        Assert.Equal(expected, total);
    }

    [Fact]
    public void Preview_SubtotalExactly1000_NoShipping()
    {
        var sut = new LegacyRegistrationProcessor();

        var total = sut.Preview("regular", 0, OneLine(1000m));

        Assert.Equal(1000m, total);
    }

    [Fact]
    public void Preview_SubtotalJustBelow1000_AddsShipping()
    {
        var sut = new LegacyRegistrationProcessor();

        var total = sut.Preview("regular", 0, OneLine(999.99m));

        Assert.Equal(1059.99m, total);
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var c = new Member
        {
            Name = " іван ",
            Email = "IVAN@MAIL.COM",
            Kind = "vip",
            DoneCount = 12,
            SinceUtc = new DateTime(2020, 1, 1)
        };
        // метод залежить від системного годинника,
        // тому очікування обчислюємо, а не вписуємо
        var years = DateTime.Now.Year - 2020;
        var expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] <ivan@mail.com> стаж " + years;

        var actual = new LegacyRegistrationProcessor().DescribeClient(c);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DumpLog_AfterSuccessWithoutMail_ContainsOnlyOkLine()
    {
        var sut = new LegacyRegistrationProcessor();
        sut.Handle(1001, 7, "Іван", Ok, "regular", 0,
            TwoLines(), "new", "UAH", Created, false);

        Assert.Equal("ok 1001\n", sut.DumpLog());
    }

    [Fact]
    public void DumpLog_AfterSuccessWithMail_ContainsMailLine()
    {
        var sut = new LegacyRegistrationProcessor();
        sut.Handle(1001, 7, "Іван", Ok, "regular", 0,
            TwoLines(), "new", "UAH", Created, true);

        Assert.Equal("ok 1001\nmail -> a@b.c\n", sut.DumpLog());
    }
}
