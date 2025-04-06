namespace SpotifyGoldServer.Api;

using Microsoft.AspNetCore.Mvc;
using SpotifyGoldServer.Utils;

[ApiController]
[Route(ROUTE)]
public class YoutubeController: ControllerBase
{
    public const string ROUTE = "api/youtube";

    [HttpGet("{id}")]
    public IActionResult Download(string id)
	{
        var url = YoutubeUtils.GetUrl(id);
        var videoPath = YoutubeUtils.DownloadVideoAsMp3(url);
        var stream = ServerUtils.ReadFile(videoPath);

        var contentType = ContentTypeEnum.AUDIO_MP3;
        var fileName = Path.GetFileName(videoPath);

        return File(stream, contentType, fileName);
    }
}
