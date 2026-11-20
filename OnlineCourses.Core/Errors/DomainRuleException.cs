namespace OnlineCourses.Core.Errors;

public sealed class DomainRuleException : InvalidOperationException
{
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        Rule = rule;
    }

    public DomainRuleException(string rule, string message, Exception inner)
        : base(message, inner)
    {
        Rule = rule;
    }

    public string Rule { get; }
}
