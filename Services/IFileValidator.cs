using System.Collections.Generic;

namespace S3FileManager.Services
{
    public interface IFileValidator
    {
        bool ValidateFile(string filePath, out List<string> errors);
        string FormatFileSize(long bytes);
    }
}

