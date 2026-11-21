namespace OnlineCourses.Core.Errors;

/// <summary>Представляє відмову операції через порушення правила предметної області.</summary>
public sealed class DomainRuleException : InvalidOperationException
{
    /// <summary>Initializes a new instance of the <see cref="DomainRuleException"/> class.</summary>
    /// <param name="rule">Стабільний код порушеного правила.</param>
    /// <param name="message">Опис причини відмови.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>Initializes a new instance of the <see cref="DomainRuleException"/> class with an inner exception.</summary>
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
