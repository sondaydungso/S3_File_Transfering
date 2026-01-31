using System;

namespace S3FileManager.Models
{
    public class S3File
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime UploadAt { get; set; }
        public string Size { get; set; }
        public string S3Key { get; set; }
    }
}

