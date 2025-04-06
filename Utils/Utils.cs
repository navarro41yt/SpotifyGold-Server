namespace SpotifyGoldServer.Utils;

using System.Diagnostics;

public static class ServerUtils
{
    private static Tuple<string, string[]> SplitCommand(string command)
    {
        var parts = command.Split(' ', 2);
        var args = Array.Empty<string>();

        if (parts.Length > 1) {
            args = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        return new Tuple<string, string[]>(parts[0], args);
    }

    public static string Exec(string command, params string[] parameters)
    {
        if (parameters.Length == 0) {
            var cmd = SplitCommand(command);
            command = cmd.Item1;
            parameters = cmd.Item2.Concat(parameters).ToArray();
        }

        var result = "";

        try {
            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = command,
                Arguments = string.Join(" ", parameters),
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using Process process = new Process { StartInfo = startInfo };
            process.Start();
            result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        } catch (Exception ex) {
            Console.WriteLine($"Error executing command: {ex.Message}");
        }

        return result;
    }

    public static Stream ReadFile(string path)
    {
        var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return fileStream;
    }
}


class YoutubeUtils
{
    public static readonly string YOUTUBE_FOLDER = Path.Combine(PathUtils.TEMP_DIR, "Youtube");
    public static readonly string YOUTUBE_AUDIO_FOLDER = Path.Combine(YOUTUBE_FOLDER, "Audio");
    public static readonly string YOUTUBE_VIDEO_FOLDER = Path.Combine(YOUTUBE_FOLDER, "Video");

    public const string FILENAME_TEMPLATE = "[SPG] [%(id)s] %(title)s.%(ext)s";

    private static string ExtractIdFromURL(string id)
    {
        var parts = id.Split("=");
        if (parts.Length > 1) {
            return parts[1];
        }

        throw new Exception($"Invalid URL: {id}");
    }

    private static string GetStartFilename(string id)
    {
        return $"[SPG] [{id}] ";
    }

    public static string GetUrl(string id)
    {
        return $"https://www.youtube.com/watch?v={id}";
    }

    public static string DownloadVideoAsMp3(string url)
    {
        var id = ExtractIdFromURL(url);
        var startFilename = GetStartFilename(id);

        var downloadFolder = YOUTUBE_AUDIO_FOLDER;

        var alreadyDownloaded = PathUtils.LsOneStart(downloadFolder, startFilename);

        if (alreadyDownloaded != null) {
            return alreadyDownloaded;
        }

        var command = $"yt-dlp -f bestaudio --extract-audio --audio-format mp3 --audio-quality 0 -P \"{YOUTUBE_AUDIO_FOLDER}\" {url} -o \"{FILENAME_TEMPLATE}\"";
        ServerUtils.Exec(command);

        var downloadedPath = PathUtils.LsOneStart(downloadFolder, startFilename);

        return downloadedPath!;
    }
}
