using System.Collections.Concurrent;

namespace CareConnect.Queries.Common;

/// <summary>Loads and caches SQL text embedded as a resource in the Queries assembly.</summary>
public static class SqlResourceLoader
{
    private static readonly ConcurrentDictionary<string, string> Cache = new();

    /// <summary>
    /// Loads <paramref name="fileName"/> from the folder <paramref name="anchor"/> lives in.
    /// </summary>
    /// <param name="anchor">
    /// Any type declared in the same folder as the .sql file — normally the handler that runs it.
    /// This relies on namespace matching folder path, which is what makes an embedded resource's
    /// manifest name "{Namespace}.{FileName}". Move a file without its handler (or rename a
    /// namespace without moving the folder) and this throws on first use.
    /// </param>
    /// <param name="fileName">e.g. "GetClientsQuery.sql".</param>
    public static string Load(Type anchor, string fileName)
    {
        var resourceName = $"{anchor.Namespace}.{fileName}";

        return Cache.GetOrAdd(resourceName, name =>
        {
            using var stream = anchor.Assembly.GetManifestResourceStream(name)
                ?? throw new InvalidOperationException(
                    $"Embedded SQL resource '{name}' was not found. The .sql file must sit in the same folder as " +
                    $"'{anchor.FullName}' and be named '{fileName}'.");
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        });
    }
}
