using Newtonsoft.Json;
using System.Text;

namespace Transliterator.Core.Services;

public static class FileService
{
    static FileService()
    {
        BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    }

    public static string BaseDirectory { get; }

    public static T Read<T>(string folderPath, string fileName)
    {
        var path = Path.Combine(folderPath, fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        return default;
    }

    public static void Save<T>(string folderPath, string fileName, T content)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = JsonConvert.SerializeObject(content);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
    }

    public static void Delete(string folderPath, string fileName)
    {
        if (fileName != null && File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Delete(Path.Combine(folderPath, fileName));
        }
    }

    // TODO: Improve?
    public static List<string> GetFilePaths(string path)
    {
        if (Directory.Exists(path))
        {
            return Directory.GetFiles(path).ToList();
        }
        else return new List<string>();
    }

    public static List<string> GetFileNamesWithoutExtension(string path)
    {
        var files = GetFilePaths(path);

        List<string> names = new List<string>();

        foreach (var file in files)
        {
            names.Add(Path.GetFileNameWithoutExtension(file));
        }

        return names;
    }
}