namespace OnlineCourses.Core.Errors;

public class DomainRuleException : Exception
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
