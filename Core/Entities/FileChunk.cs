using System;
using System.Collections.Generic;
using System.Text;

namespace AssetTracker.Core.Entities
{
    public class FileChunk
    {
        public Guid Id { get; set; }

        public Guid FileId { get; set; }

        public int ChunkIndex { get; set; }

        public byte[] Data { get; set; }

        public int Size { get; set; }

        public DateTime CreatedAt { get; set; }

        public FileMetadata File { get; set; }
    }
}
