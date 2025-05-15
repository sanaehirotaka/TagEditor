using System.Text.Json;

namespace TagEditor;

public class Storage
{
    private static readonly string STORAGE_DIR = Environment.GetEnvironmentVariable("STORAGE_DIR")!;

    public T? Get<T>(string? name = null)
    {
        try
        {
            using var stream = File.OpenRead(GetPath<T>(name));
            return JsonSerializer.Deserialize<T>(stream);
        }
        catch
        {
            return default;
        }
    }

    public void Set<T>(T value, string? name = null)
    {
        using var stream = File.Create(GetPath<T>(name));
        JsonSerializer.Serialize(stream, value);
    }

    private string GetPath<T>(string? name)
    {
        if (name == null)
        {
            return Path.Combine(STORAGE_DIR, typeof(T).Name + ".json");
        }
        else
        {
            return Path.Combine(STORAGE_DIR, typeof(T).Name + name + ".json");
        }
    }
}
