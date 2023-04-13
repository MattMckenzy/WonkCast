﻿using Homehook.Attributes;
using Homehook.Models;
using Homehook.Models.Jellyfin;
using Homehook.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using WonkCast.Common.Models;

namespace Homehook.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JellyController : ControllerBase
    {
        private JellyfinService JellyfinService { get; }
        private LanguageService LanguageService { get; }
        private CastService CastService { get; }
        private LoggingService<JellyController> LoggingService { get; }
        private IConfiguration Configuration { get; }

        public JellyController(JellyfinService jellyfinService, LanguageService languageService, CastService castService, LoggingService<JellyController> loggingService, IConfiguration configuration)
        {
            JellyfinService = jellyfinService;
            LanguageService = languageService;
            CastService = castService;
            LoggingService = loggingService;
            Configuration = configuration;
        }

        [HttpGet("{receiver}/toggleplayback")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> TogglePlayback([FromRoute] string deviceName)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            if (deviceConnection.Device.DeviceStatus == DeviceStatus.Playing ||
                deviceConnection.Device.DeviceStatus == DeviceStatus.Unpausing)
                await deviceConnection.HubConnection.InvokeAsync("Pause");
            else if (deviceConnection.Device.DeviceStatus == DeviceStatus.Paused ||
                deviceConnection.Device.DeviceStatus == DeviceStatus.Pausing)
                await deviceConnection.HubConnection.InvokeAsync("Play");

            return Ok();
        }

        [HttpGet("{receiver}/togglemute")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> ToggleMute([FromRoute] string deviceName)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            await deviceConnection.HubConnection.InvokeAsync("ToggleMute");

            return Ok();
        }

        [HttpGet("{receiver}/next")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> Next([FromRoute] string deviceName)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            await deviceConnection.HubConnection.InvokeAsync("Next");

            return Ok();
        }


        [HttpGet("{receiver}/previous")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> Previous([FromRoute] string deviceName)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            await deviceConnection.HubConnection.InvokeAsync("Previous");

            return Ok();
        }

        [HttpGet("{receiver}/stop")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> Stop([FromRoute] string deviceName)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            await deviceConnection.HubConnection.InvokeAsync("Stop");

            return Ok();
        }


        [HttpGet("{receiver}/seek")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> Seek([FromRoute] string deviceName, [FromQuery]int seconds = 30)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            await deviceConnection.HubConnection.InvokeAsync("Seek", deviceConnection.Device.CurrentTime + seconds);

            return Ok();
        }

        [HttpGet("{receiver}/volume")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> Volume([FromRoute] string deviceName, [FromQuery] double change = 10)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            await deviceConnection.HubConnection.InvokeAsync("SetVolume", deviceConnection.Device.Volume + ((double)change / 100));

            return Ok();
        }
        

        [HttpGet("{receiver}/playbackrate")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> PlaybackRate([FromRoute] string deviceName, [FromQuery] double change = 0.5)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            await deviceConnection.HubConnection.InvokeAsync("PlaybackRate", deviceConnection.Device.PlaybackRate + change);

            return Ok();
        }

        [HttpGet("{receiver}/shuffle")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> Shuffle([FromRoute] string deviceName)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            await deviceConnection.HubConnection.InvokeAsync("ShuffleQueue");
            await deviceConnection.HubConnection.InvokeAsync("ChangeRepeatMode", RepeatMode.All);

            return Ok();
        }

        [HttpGet("{receiver}/repeatone")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> RepeatOne([FromRoute] string deviceName)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            await deviceConnection.HubConnection.InvokeAsync("ChangeRepeatMode", RepeatMode.One);

            return Ok();
        }

        [HttpGet("{receiver}/repeatall")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> RepeatAll([FromRoute] string deviceName)
        {
            (bool getSuccess, DeviceConnection? deviceConnection) = await CastService.TryGetDevice(deviceName);

            if (!getSuccess || deviceConnection == null)
                return NotFound($"Requested device \"{deviceName}\" not found!");

            await deviceConnection.HubConnection.InvokeAsync("ChangeRepeatModeAsync", RepeatMode.All);

            return Ok();
        }

        [HttpPost("simple")]
        [ApiKey(ApiKeyName = "apiKey", ApiKeysRoute = "Services:Homehook:Tokens")]
        public async Task<IActionResult> PostJellySimpleHook([FromBody] SimplePhrase simplePhrase)
        {
            JellyPhrase phrase = await LanguageService.ParseJellyfinSimplePhrase(simplePhrase.Content);
            await LoggingService.LogDebug("PostJellySimpleHook - parsed phrase.", $"Succesfully parsed the following phrase from the search term: {simplePhrase.Content}" , phrase);

            if (string.IsNullOrWhiteSpace(phrase.SearchTerm))
                return BadRequest("Missing search content! Please specify a search term along with any optional filters.");

            return await ProcessJellyPhrase(phrase, nameof(PostJellySimpleHook));
        }

        private async Task<IActionResult> ProcessJellyPhrase(JellyPhrase phrase, string controllerName)
        {
            phrase.UserId = await JellyfinService.GetUserId(phrase.User);
            if (string.IsNullOrWhiteSpace(phrase.UserId))
            {
                await LoggingService.LogWarning($"{controllerName} - no user found", $"{phrase.SearchTerm}, or the default user, returned no available user IDs.", phrase);
                return BadRequest($"No user found! - {phrase.SearchTerm}, or the default user, returned no available user Ids");
            }

            List<Media> queueItems = await JellyfinService.GetItems(phrase);
            await LoggingService.LogDebug($"{controllerName} - items found.", $"Found {queueItems.Count} item(s) with the search term {phrase.SearchTerm}.");
            await LoggingService.LogInformation($"{controllerName} - items found.", "Found the following items:", queueItems);
            if (!queueItems.Any())
            {
                await LoggingService.LogWarning($"{controllerName} - no results", $"{phrase.SearchTerm} returned no search results.", phrase);
                return NotFound($"No user found! - {phrase.SearchTerm}, or the default user, returned no available user IDs.");
            }
            
            _ = Task.Run(async () => await CastService.StartJellyfinSession(phrase.Device, queueItems));

            return Ok($"Found {queueItems.Count} item(s) with the search term {phrase.SearchTerm}.");
        }
    }
}
  