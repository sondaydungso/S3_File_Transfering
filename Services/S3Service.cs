using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using S3FileManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3FileManager.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 s3Client;
        private readonly string bucketName;
        private readonly string user_id;

        public S3Service(string user_id, string bucketName = null, string region = null)
        {
            if (string.IsNullOrWhiteSpace(user_id))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(user_id));
            }

            this.user_id = user_id;
            var credentials = new AppCredentials();
            this.bucketName = credentials.BucketName() ?? bucketName;

            var regionEndpoint = credentials.Region() != null
                ? Amazon.RegionEndpoint.GetBySystemName(credentials.Region())
                : (region != null ? Amazon.RegionEndpoint.GetBySystemName(region) : Amazon.RegionEndpoint.USEast1);

            var accessKeyId = credentials.GetAccessKeyId();
            var secretAccessKey = credentials.GetSecretAccessKey();

            if (string.IsNullOrEmpty(accessKeyId) || string.IsNullOrEmpty(secretAccessKey))
            {
                throw new InvalidOperationException(
                    "AWS credentials not found." );
            }

            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(accessKeyId, secretAccessKey);
            s3Client = new AmazonS3Client(awsCredentials, regionEndpoint);
        }

        private string GetUserPrefixedKey(string key)
        {
            return $"{user_id}/{key}";
        }

        private string ExtractFileName(string s3Key)
        {
            if (s3Key.StartsWith($"{user_id}/"))
            {
                return s3Key.Substring(user_id.Length + 1);
            }
            return Path.GetFileName(s3Key);
        }

        public async Task<List<S3File>> ListFilesAsync()
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(user_id))
                {
                    throw new InvalidOperationException("User ID is not set. Cannot list files.");
                }

                var prefix = $"{user_id}/";
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    Prefix = prefix 
                };

                var response = await s3Client.ListObjectsV2Async(request);
                var files = new List<S3File>();

                if (response.S3Objects != null)
                {
                    foreach (var s3Object in response.S3Objects)
                    {
                        // Skip if it's just the prefix itself (folder marker)
                        if (s3Object.Key == prefix || s3Object.Key.EndsWith("/"))
                        {
                            continue;
                        }

                        // Only include files that match the user prefix
                        if (s3Object.Key.StartsWith(prefix))
                        {
                            var fileName = ExtractFileName(s3Object.Key);
                            
                            // Skip if filename is empty (shouldn't happen, but safety check)
                            if (string.IsNullOrEmpty(fileName))
                            {
                                continue;
                            }

                            files.Add(new S3File
                            {
                                FileName = fileName,
                                S3Key = s3Object.Key,
                                FileSize = s3Object.Size ?? 0,
                                LastModified = s3Object.LastModified ?? DateTime.MinValue,
                                UploadAt = s3Object.LastModified ?? DateTime.MinValue,
                                FileType = Path.GetExtension(fileName),
                                Size = FormatFileSize(s3Object.Size ?? 0)
                            });
                        }
                    }
                }

                return files;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error listing files from S3: {ex.Message}", ex);
            }
        }

        public async Task<bool> UploadFileAsync(string filePath, IProgress<int> progress)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}");
                }

                var fileName = Path.GetFileName(filePath);
                var key = GetUserPrefixedKey(fileName);

                var fileTransferUtility = new TransferUtility(s3Client);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    FilePath = filePath
                };

                uploadRequest.UploadProgressEvent += (sender, e) =>
                {
                    var percentage = (int)((double)e.TransferredBytes / e.TotalBytes * 100);
                    progress?.Report(percentage);
                };

                await fileTransferUtility.UploadAsync(uploadRequest);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file to S3: {ex.Message}", ex);
            }
        }

        public async Task<bool> DownloadFileAsync(string s3Key, string destinationPath)
        {
            try
            {
                var directory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var fileTransferUtility = new TransferUtility(s3Client);
                await fileTransferUtility.DownloadAsync(destinationPath, bucketName, s3Key);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading file from S3: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteFileAsync(string s3Key)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = s3Key
                };

                await s3Client.DeleteObjectAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file from S3: {ex.Message}", ex);
            }
        }

        public Task<string> GenerateDownloadUrlAsync(string s3Key, int expiryMinutes = 60)
        {
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = s3Key,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
                };

                var url = s3Client.GetPreSignedURL(request);
                return Task.FromResult(url);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating download URL: {ex.Message}", ex);
            }
        }

        private string FormatFileSize(long bytes)
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

        public void Dispose()
        {
            s3Client?.Dispose();
        }
    }
}

