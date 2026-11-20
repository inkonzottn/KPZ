namespace OnlineCourses.Tests;

using OnlineCourses.Core.Domain;

public sealed class ArchitectureTests
{
    [Fact]
    public void Core_DoesNotReferencePresentationOrTests()
    {
        var references = typeof(Course).Assembly
            .GetReferencedAssemblies()
            .Select(x => x.Name)
            .ToArray();

        Assert.DoesNotContain("OnlineCourses.App", references);
        Assert.DoesNotContain("OnlineCourses.Tests", references);
    }

    [Fact]
    public void DomainPublicMembers_DoNotDependOnInfrastructure()
    {
        var domainTypes = typeof(Course).Assembly.GetTypes()
            .Where(x => x.Namespace == typeof(Course).Namespace);

        var memberTypes = domainTypes
            .SelectMany(x => x.GetMembers())
            .OfType<System.Reflection.MethodBase>()
            .SelectMany(x => x.GetParameters())
            .Select(x => x.ParameterType)
            .Concat(domainTypes.SelectMany(x => x.GetProperties()).Select(x => x.PropertyType));

        Assert.DoesNotContain(
            memberTypes,
            x => x.FullName?.Contains(".Infrastructure.", StringComparison.Ordinal) == true);
    }
}
