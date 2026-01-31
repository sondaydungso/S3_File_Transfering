using S3FileManager.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace S3FileManager.Services
{
    public class FileValidator : IFileValidator
    {
        private readonly AppSettings settings;

        public FileValidator(AppSettings settings)
        {
            this.settings = settings;
        }

        public bool ValidateFile(string filePath, out List<string> errors)
        {
            errors = new List<string>();

            if (!File.Exists(filePath))
            {
                errors.Add("File does not exist");
                return false;
            }

            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Length > settings.MaxFileSizeBytes)
            {
                errors.Add($"File size ({FormatFileSize(fileInfo.Length)}) exceeds maximum allowed size ({FormatFileSize(settings.MaxFileSizeBytes)})");
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (!settings.AllowedFileTypes.Contains(extension))
            {
                errors.Add($"File type {extension} is not allowed. Allowed types: {string.Join(", ", settings.AllowedFileTypes)}");
            }

            return errors.Count == 0;
        }

        public string FormatFileSize(long bytes)
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

