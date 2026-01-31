using System;

namespace S3FileManager.UI
{
    public class ProgressBar
    {
        private readonly int barWidth;
        private long totalBytes;
        private long transferredBytes;

        public ProgressBar(int barWidth = 25)
        {
            this.barWidth = barWidth;
        }

        public void SetTotalBytes(long totalBytes)
        {
            this.totalBytes = totalBytes;
        }

        public void Update(long transferredBytes, long totalBytes)
        {
            this.transferredBytes = transferredBytes;
            this.totalBytes = totalBytes;
            Draw();
        }

        public void Update(int percentage)
        {
            if (totalBytes > 0)
            {
                transferredBytes = (long)(totalBytes * percentage / 100.0);
            }
            Draw(percentage);
        }

        private void Draw(int? percentage = null)
        {
            var percent = percentage ?? (totalBytes > 0 ? (int)((double)transferredBytes / totalBytes * 100) : 0);
            var filled = (int)(barWidth * percent / 100.0);
            var empty = barWidth - filled;

            var bar = new string('█', filled) + new string('░', empty);
            
            var transferredStr = FormatBytes(transferredBytes);
            var totalStr = FormatBytes(totalBytes);

            Console.Write($"\r[{bar}] {percent}% ({transferredStr} / {totalStr})");
        }

        public void Complete()
        {
            var bar = new string('█', barWidth);
            var totalStr = FormatBytes(totalBytes);
            Console.Write($"\r[{bar}] 100% ({totalStr} / {totalStr})");
            Console.WriteLine();
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}

