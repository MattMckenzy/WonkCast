﻿namespace HomeHook.Common.Models
{
    public class MediaItem
    {
        public required string Id { get; set; }
        public required string Location { get; set; }
        public required MediaItemKind MediaItemKind { get; set; }
        public required MediaMetadata Metadata { get; set; }
        public string User { get; set; } = string.Empty;
        public bool Cache { get; set; } = false;
        public float StartTime { get; set; } = 0;
        public float Runtime { get; set; } = 0;
    }
}
