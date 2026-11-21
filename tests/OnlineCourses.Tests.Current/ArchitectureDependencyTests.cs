namespace OnlineCourses.Tests;

using System.Reflection;
using OnlineCourses.Core.Domain;

public sealed class ArchitectureDependencyTests
{
    [Fact]
    public void CoreAssembly_DoesNotReferenceConsoleOrTestProjects()
    {
        var references = typeof(Enrollment).Assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .ToArray();

        Assert.DoesNotContain("OnlineCourses.App", references);
        Assert.DoesNotContain("OnlineCourses.Tests", references);
    }

    [Fact]
    public void DomainTypes_DoNotDependOnApplicationOrInfrastructureNamespaces()
    {
        var domainTypes = typeof(Enrollment).Assembly.GetTypes()
            .Where(type => type.Namespace == "OnlineCourses.Core.Domain");

        var forbiddenReferences = domainTypes
            .SelectMany(GetMemberTypes)
            .Where(type =>
                type.Namespace == "OnlineCourses.Core.Services" ||
                type.Namespace == "OnlineCourses.Core.Infrastructure")
            .Distinct()
            .ToArray();

        Assert.Empty(forbiddenReferences);
    }

    private static IEnumerable<Type> GetMemberTypes(Type type)
    {
        const BindingFlags flags =
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.DeclaredOnly;

        foreach (var field in type.GetFields(flags))
        {
            foreach (var referencedType in ExpandType(field.FieldType))
            {
                yield return referencedType;
            }
        }

        foreach (var property in type.GetProperties(flags))
        {
            foreach (var referencedType in ExpandType(property.PropertyType))
            {
                yield return referencedType;
            }
        }

        foreach (var method in type.GetMethods(flags).Cast<MethodBase>()
            .Concat(type.GetConstructors(flags)))
        {
            if (method is MethodInfo methodInfo)
            {
                foreach (var referencedType in ExpandType(methodInfo.ReturnType))
                {
                    yield return referencedType;
                }
            }

            foreach (var parameter in method.GetParameters())
            {
                foreach (var referencedType in ExpandType(parameter.ParameterType))
                {
                    yield return referencedType;
                }
            }
        }
    }

    private static IEnumerable<Type> ExpandType(Type type)
    {
        if (type.HasElementType && type.GetElementType() is { } elementType)
        {
            foreach (var referencedType in ExpandType(elementType))
            {
                yield return referencedType;
            }

            yield break;
        }

        yield return type;

        if (type.IsGenericType)
        {
            foreach (var argument in type.GetGenericArguments())
            {
                foreach (var referencedType in ExpandType(argument))
                {
                    yield return referencedType;
                }
            }
        }
    }
}
