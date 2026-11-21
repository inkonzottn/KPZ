namespace OnlineCourses.Core.Errors;

/// <summary>Представляє відмову операції через порушення правила предметної області.</summary>
public sealed class DomainRuleException : InvalidOperationException
{
    /// <summary>Створює виняток із кодом правила та зрозумілим повідомленням.</summary>
    /// <param name="rule">Стабільний код порушеного правила.</param>
    /// <param name="message">Опис причини відмови.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>Створює виняток із кодом правила, повідомленням і внутрішньою помилкою.</summary>
    /// <param name="rule">Стабільний код порушеного правила.</param>
    /// <param name="message">Опис причини відмови.</param>
    /// <param name="inner">Початкова помилка, що спричинила відмову.</param>
    public DomainRuleException(string rule, string message, Exception inner)
        : base(message, inner)
    {
        Rule = rule;
    }

    /// <summary>Gets the stable identifier of the violated domain rule.</summary>
    public string Rule { get; }
}
