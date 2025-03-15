using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NLog;
using QP.Storage.WebApp.Settings;

namespace QP.Storage.WebApp.Middleware;

public class ReduceSettingsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ImageResizeSettings _settings;
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly JsonSerializerOptions _serializerOptions;

    public ReduceSettingsMiddleware(RequestDelegate next, IOptions<ImageResizeSettings> settings)
    {
        _next = next;
        _settings = settings.Value;

        _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task Invoke(HttpContext context)
    {
        string basePath = _settings.BasePath.StartsWith("/") ?
            _settings.BasePath :
            $"/{_settings.BasePath}";

        if (context.Request.Path.StartsWithSegments(basePath))
        {
            _logger.Log(LogLevel.Debug, $"Call to {_settings.BasePath}");
            await context.Response.WriteAsync(JsonSerializer.Serialize(_settings, _serializerOptions));

            return;
        }

        await _next(context);
    }
}
