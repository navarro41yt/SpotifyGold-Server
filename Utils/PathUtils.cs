namespace SpotifyGoldServer.Utils;

public class PathUtils
{
    public static readonly string ROOT = Directory.GetCurrentDirectory();
    public static readonly string TEMP_DIR = Path.Combine(ROOT, "Temp");

    public static void CleanTempFolder(TimeSpan maxTime)
    {
        var now = DateTime.Now;

        var files = Directory.GetFiles(TEMP_DIR);
        foreach (var file in files) {
            var creationTime = File.GetCreationTime(file);
            var fileAge = now - creationTime;

            if (fileAge > maxTime) {
                File.Delete(file);
            }
        }

        var dirs = Directory.GetDirectories(TEMP_DIR);
        foreach (var dir in dirs) {
            var creationTime = Directory.GetCreationTime(dir);
            var dirAge = now - creationTime;

            if (dirAge > maxTime) {
                Directory.Delete(dir, recursive: true);
            }
        }
    }

    public static void CreatePathIfNotExists(string path)
    {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        if (!File.Exists(path)) {
            File.Create(path).Close();
        }
    }

    public static string[] LsRegex(string dir, string pattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        if (!Directory.Exists(dir)) {
            return [];
        }

        var files = Directory.GetFiles(dir, pattern, searchOption);
        return files;
    }

    public static string[] LsStart(string dir, string starts, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var pattern = $"{starts}*";
        return LsRegex(dir, pattern, searchOption);
    }

    public static string[] LsEnd(string dir, string ends, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var pattern = $"*{ends}";
        return LsRegex(dir, pattern, searchOption);
    }

    public static string[] LsContains(string dir, string contains, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var pattern = $"*{contains}*";
        return LsRegex(dir, pattern, searchOption);
    }

    public static string? LsOneRegex(string dir, string pattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var files = LsRegex(dir, pattern, searchOption);
        if (files.Length == 0) {
            return null;
        }
        if (files.Length > 1) {
            throw new MultipleFilesFoundException(dir, pattern, files);
        }
        return files[0];
    }

    public static string? LsOneStart(string dir, string starts, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var pattern = $"{starts}*";
        return LsOneRegex(dir, pattern, searchOption);
    }

    public static string? LsOneEnd(string dir, string ends, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var pattern = $"*{ends}";
        return LsOneRegex(dir, pattern, searchOption);
    }

    public static string? LsOneContains(string dir, string contains, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var pattern = $"*{contains}*";
        return LsOneRegex(dir, pattern, searchOption);
    }
}


public class MultipleFilesFoundException(string searchDir, string pattern, string[] files)
    : Exception(FormatMessage(searchDir, pattern, files))
{
    public static string FormatMessage(string searchDir, string pattern, string[] files)
    {
        var fileList = string.Join("\n", files);
        return $"Found multiple files matching the pattern '{pattern}' in directory '{searchDir}':\n{fileList}";
    }
}
