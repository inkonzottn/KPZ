using System;
using System.Collections.Generic;
namespace OnlineCourses.src.Legacy;

public enum ClientKind { Regular, Vip, Staff }
public enum DocState { Draft, New, Paid, Closed }

public sealed class Member
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Email { get; init; } = "";
    public ClientKind Kind { get; init; }
    public int DoneCount { get; init; }
    public DateTime SinceUtc { get; init; }
    public bool HasValidEmail =>
        Email is not null && Email.Contains("@");
    public bool IsLoyal =>
        DoneCount > PricingRules.LoyalOrders;
}

public class RegistrationItem
{
    public string Code = "";
    public int Qty;
    public decimal Price;
    public string Group = "";
}

public class LegacyRegistrationProcessor
{
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly List<string> _log = new();

    public string Handle(OrderRequest req)
    {
        var error = Validate(req);
        if (error != null)
        {
            return error;
        }

        _log.Add("ok " + req.DocId);

        _tmpSum = Subtotal(req.Items!);
        _tmpDiscount = 0m;
        if (req.Client.Kind == ClientKind.Vip)
        {
            _tmpDiscount = _tmpSum * PricingRules.VipRate;
            if (_tmpDiscount > PricingRules.MaxDiscount)
            {
                _tmpDiscount = PricingRules.MaxDiscount;
            }
        }
        else if (req.Client.Kind == ClientKind.Staff)
        {
            _tmpDiscount = _tmpSum * PricingRules.StaffRate;
            if (_tmpDiscount > PricingRules.MaxDiscount)
            {
                _tmpDiscount = PricingRules.MaxDiscount;
            }
        }
        else if (req.Client.DoneCount > PricingRules.LoyalOrders)
        {
            _tmpDiscount = _tmpSum * PricingRules.LoyalRate;
            if (_tmpDiscount > PricingRules.MaxDiscount)
            {
                _tmpDiscount = PricingRules.MaxDiscount;
            }
        }
        decimal ship = 0m;
        if (_tmpSum - _tmpDiscount < PricingRules.FreeShipFrom)
        {
            ship = PricingRules.ShipCost;
        }

        decimal total = _tmpSum - _tmpDiscount + ship;
        string txt = "Документ #" + req.DocId + "\n";
        txt += "Клієнт: " + req.Client.Name + "\n";
        for (int i = 0; i < req.Items!.Count; i++)
        {
            txt += req.Items[i].Code + " x" + req.Items[i].Qty + " = " +
            (req.Items[i].Qty * req.Items[i].Price)
            .ToString("0.00") + " " + req.Currency + "\n";
        }
        txt += "Знижка: " + _tmpDiscount.ToString("0.00") +
        " " + req.Currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
        " " + req.Currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
        " " + req.Currency + "\n";
        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Client.Email);
        }
        return txt;
    }


    public decimal Preview(Member client, IReadOnlyList<RegistrationItem> items)
    {
        decimal s = Subtotal(items);
        decimal d = 0m;
        if (client.Kind == ClientKind.Vip)
        {
            d = s * PricingRules.VipRate;
            if (d > PricingRules.MaxDiscount)
            {
                d = PricingRules.MaxDiscount;
            }
        }
        else if (client.Kind == ClientKind.Staff)
        {
            d = s * PricingRules.StaffRate;
            if (d > PricingRules.MaxDiscount)
            {
                d = PricingRules.MaxDiscount;
            }
        }
        else if (client.DoneCount > PricingRules.LoyalOrders)
        {
            d = s * PricingRules.LoyalRate;
            if (d > PricingRules.MaxDiscount)
            {
                d = PricingRules.MaxDiscount;
            }
        }
        decimal sh = 0m;
        if (s - d < PricingRules.FreeShipFrom)
        {
            sh = PricingRules.ShipCost;
        }

        return s - d + sh;
    }

    public string DescribeClient(Member c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == ClientKind.Vip)
        {
            s += " [VIP]";
        }

        if (c.DoneCount > PricingRules.LoyalOrders)
        {
            s += " [ЛОЯЛЬНИЙ]";
        }

        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }
    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log)
        {
            r += l + "\n";
        }

        return r;
    }

    private static string? Validate(OrderRequest req)
    {
        if (req.Items is null)
        {
            return "ERR: null";
        }

        if (req.Items.Count == 0)
        {
            return "ERR: empty";
        }

        if (req.State is not (DocState.New or DocState.Paid))
        {
            return "ERR: state";
        }

        if (!req.Client.HasValidEmail)
        {
            return "ERR: mail";
        }

        return null;
    }

    private static decimal Subtotal(
        IReadOnlyList<RegistrationItem> items)
    {
        decimal sum = 0m;
        foreach (var it in items)
        {
            sum += it.Qty * it.Price;
        }

        return sum;
    }
}
