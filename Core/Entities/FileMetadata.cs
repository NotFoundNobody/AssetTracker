using System;
using System.Collections.Generic;
using System.Text;

namespace AssetTracker.Core.Entities
{
    public class FileMetadata
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public long FileSize { get; set; }

        public int TotalChunks { get; set; }

        public string Checksum { get; set; }

        public DateTime UploadDate { get; set; }

        public bool IsCompleted { get; set; }

        public ICollection<FileChunk> Chunks { get; set; } = new List<FileChunk>();
    }
}

