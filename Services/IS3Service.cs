using S3FileManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S3FileManager.Services
{
    public interface IS3Service
    {
        Task<List<S3File>> ListFilesAsync();
        Task<string> UploadFileAsync(string filePath, IProgress<int> progress);
        Task<bool> DownloadFileAsync(string s3Key, string destinationPath);
        Task<bool> DeleteFileAsync(string s3Key);
        Task<string> GenerateDownloadUrlAsync(string s3Key, int expiryMinutes);
    }
}

