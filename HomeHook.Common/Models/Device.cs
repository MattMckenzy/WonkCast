﻿using System.Text.Json.Serialization;

namespace HomeHook.Common.Models
{
    public class Device
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string Version { get; set; }

        public DeviceStatus DeviceStatus { get; set; } = DeviceStatus.Stopped;
        public string? CurrentMediaItemId { get; set; } = null;
        public List<MediaItem> MediaQueue { get; set; } = new List<MediaItem>();

        public double CurrentTime { get; set; }
        public float Volume { get; set; } = 0.5f;
        public bool IsMuted { get; set; } = false;
        public float PlaybackRate { get; set; } = 1;
        public string? StatusMessage { get; set; }
        public RepeatMode RepeatMode { get; set; } = RepeatMode.Off;

        [JsonIgnore]
        public MediaItem? CurrentMedia { get { return CurrentMediaItemId == null ? null : MediaQueue.FirstOrDefault(mediaItem => CurrentMediaItemId == mediaItem.Id); } }
    }
}
