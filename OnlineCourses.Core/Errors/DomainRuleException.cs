namespace OnlineCourses.Core.Errors;

/// <summary>Represents the DomainRuleException type.</summary>
public sealed class DomainRuleException : InvalidOperationException
{
    /// <summary>Initializes a new instance of the <see cref="DomainRuleException"/> class.</summary>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    /// <summary>Initializes a new instance of the <see cref="DomainRuleException"/> class.</summary>
    public DomainRuleException(string rule, string message, Exception inner)
        : base(message, inner)
    {
        Rule = rule;
    }

    /// <summary>Gets the Rule value.</summary>
    public string Rule { get; }
}
