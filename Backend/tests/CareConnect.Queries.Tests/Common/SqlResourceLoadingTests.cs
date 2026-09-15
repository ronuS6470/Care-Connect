using System.Reflection;
using System.Runtime.CompilerServices;
using CareConnect.Queries;
using FluentAssertions;
using Xunit;

namespace CareConnect.Queries.Tests.Common;

/// <summary>
/// Query SQL is loaded in static initializers, so a misnamed or misplaced .sql file compiles fine
/// and only throws on the first request that touches that handler. These tests move that failure
/// to test time.
/// </summary>
public class SqlResourceLoadingTests
{
    private static readonly Assembly QueriesAssembly = typeof(QueriesAssemblyMarker).Assembly;

    [Fact]
    public void EveryStaticInitializer_LoadsItsSql()
    {
        var failures = new List<string>();

        foreach (var type in QueriesAssembly.GetTypes().Where(t => !t.IsGenericTypeDefinition))
        {
            try
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
            catch (TypeInitializationException ex)
            {
                failures.Add($"{type.FullName}: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        failures.Should().BeEmpty("every SqlResourceLoader.Load call must name a .sql file that exists beside its anchor type");
    }

    [Fact]
    public void EveryEmbeddedSqlFile_SitsInAFolderWhoseNamespaceHasCode()
    {
        // SqlResourceLoader resolves "{anchor.Namespace}.{fileName}", which only works while each
        // folder's namespace matches its path. A .sql file in a folder with no types in the matching
        // namespace can never be loaded — it was moved, or its handler's namespace drifted.
        var namespaces = QueriesAssembly.GetTypes().Select(t => t.Namespace).ToHashSet();

        var orphans = QueriesAssembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".sql", StringComparison.Ordinal))
            .Where(name => !namespaces.Contains(name[..name[..^".sql".Length].LastIndexOf('.')]))
            .ToList();

        orphans.Should().BeEmpty();
    }
}
