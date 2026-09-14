using System.Collections.Concurrent;
using System.Reflection;

namespace CareConnect.Queries.Common;

/// <summary>Loads and caches SQL text embedded as a resource in a Queries-layer assembly.</summary>
public static class SqlResourceLoader
{
    private static readonly ConcurrentDictionary<string, string> Cache = new();

    /// <param name="relativeResourcePath">
    /// Dotted path under the assembly's default namespace, e.g. "CareTasks.Repositories.Sql.GetById.sql".
    /// </param>
    public static string Load(Assembly assembly, string relativeResourcePath)
    {
        var resourceName = $"{assembly.GetName().Name}.{relativeResourcePath}";

        return Cache.GetOrAdd(resourceName, name =>
        {
            using var stream = assembly.GetManifestResourceStream(name)
                ?? throw new InvalidOperationException(
                    $"Embedded SQL resource '{name}' was not found in assembly '{assembly.FullName}'.");
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        });
    }
}
