using AssetTracker.Core.Entities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Files:BaseEntity
    {
        public required Guid FilesID { get; set; } = Guid.CreateVersion7();
        public required string OriginalFileName { get; set; }

        public required MimeType MimeType { get;  set; }

        public required long Size { get;  set; }


        private readonly List<FileChunk> _chunks = new();

        public IReadOnlyCollection<FileChunk> Chunks => _chunks;


    }
}
