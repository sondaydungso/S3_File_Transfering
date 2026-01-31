using S3FileManager.Models;
using S3FileManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace S3FileManager.UI
{
    public class MenuManager
    {
        private readonly IS3Service s3Service;
        private readonly IFileValidator fileValidator;
        private readonly AppSettings settings;

        public MenuManager(IS3Service s3Service, IFileValidator fileValidator, AppSettings settings)
        {
            this.s3Service = s3Service;
            this.fileValidator = fileValidator;
            this.settings = settings;
        }

        public void ShowMainMenu()
        {
            ConsoleHelper.ClearScreen();
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine("    S3 FILE MANAGER - Console Edition");
            Console.WriteLine("═══════════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("1. List Files");
            Console.WriteLine("2. Upload File");
            Console.WriteLine("3. Download File");
            Console.WriteLine("4. Delete File");
            Console.WriteLine("5. Generate Share Link");
            Console.WriteLine("6. Settings");
            Console.WriteLine("7. Exit");
            Console.WriteLine();
            Console.Write("Enter choice (1-7): ");
        }

        public async Task HandleMenuChoice(int choice)
        {
            try
            {
                switch (choice)
                {
                    case 1:
                        await ListFiles();
                        break;
                    case 2:
                        await UploadFile();
                        break;
                    case 3:
                        await DownloadFile();
                        break;
                    case 4:
                        await DeleteFile();
                        break;
                    case 5:
                        await GenerateShareLink();
                        break;
                    case 6:
                        ShowSettings();
                        break;
                    case 7:
                        Console.WriteLine("Goodbye!");
                        Environment.Exit(0);
                        break;
                    default:
                        ConsoleHelper.PrintError("Invalid choice. Please select 1-7.");
                        ConsoleHelper.WaitForAnyKey();
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
                ConsoleHelper.WaitForAnyKey();
            }
        }

        private async Task ListFiles()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.PrintHeader("LIST FILES");

            var files = await s3Service.ListFilesAsync();

            if (files.Count == 0)
            {
                ConsoleHelper.PrintInfo("No files found in S3 bucket.");
            }
            else
            {
                Console.WriteLine($"Found {files.Count} file(s):");
                Console.WriteLine();
                Console.WriteLine($"{"File Name",-40} {"Size",-15} {"Type",-10} {"Last Modified",-20}");
                Console.WriteLine(new string('-', 85));

                foreach (var file in files)
                {
                    Console.WriteLine($"{file.FileName,-40} {file.Size,-15} {file.FileType,-10} {file.LastModified:yyyy-MM-dd HH:mm:ss}");
                }
            }

            ConsoleHelper.WaitForAnyKey();
        }

        private async Task UploadFile()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.PrintHeader("UPLOAD FILE");

            Console.Write("Enter file path(s) (comma or space separated): ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                ConsoleHelper.PrintError("File path cannot be empty.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            
            var filePaths = new List<string>();
            var separators = new[] { ',', ';' };
            var parts = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            
            if (parts.Length == 1)
            {
                
                var trimmed = input.Trim().Trim('"');
                if (File.Exists(trimmed))
                {
                    filePaths.Add(trimmed);
                }
                else
                {
                    
                    var spaceParts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in spaceParts)
                    {
                        var trimmedPart = part.Trim().Trim('"');
                        if (!string.IsNullOrWhiteSpace(trimmedPart))
                        {
                            filePaths.Add(trimmedPart);
                        }
                    }
                }
            }
            else
            {
                foreach (var part in parts)
                {
                    var trimmedPart = part.Trim().Trim('"');
                    if (!string.IsNullOrWhiteSpace(trimmedPart))
                    {
                        filePaths.Add(trimmedPart);
                    }
                }
            }

            if (filePaths.Count == 0)
            {
                ConsoleHelper.PrintError("No valid file paths entered.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Validating files...");

            var validFiles = new List<string>();
            var invalidFiles = new List<(string path, List<string> errors)>();

            foreach (var filePath in filePaths)
            {
                if (!fileValidator.ValidateFile(filePath, out var errors))
                {
                    invalidFiles.Add((filePath, errors));
                }
                else
                {
                    validFiles.Add(filePath);
                }
            }

            
            if (invalidFiles.Count > 0)
            {
                Console.WriteLine();
                foreach (var (path, errors) in invalidFiles)
                {
                    ConsoleHelper.PrintError($"Invalid file: {path}");
                    foreach (var error in errors)
                    {
                        ConsoleHelper.PrintError($"  - {error}");
                    }
                }
            }

            if (validFiles.Count == 0)
            {
                ConsoleHelper.PrintError("No valid files to upload.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            Console.WriteLine();
            ConsoleHelper.PrintSuccess($"Found {validFiles.Count} valid file(s) to upload:");
            foreach (var filePath in validFiles)
            {
                var fileInfo = new FileInfo(filePath);
                ConsoleHelper.PrintInfo($"  - {fileInfo.Name} ({fileValidator.FormatFileSize(fileInfo.Length)})");
            }

            Console.WriteLine();
            Console.WriteLine("Uploading to S3...");

            var successCount = 0;
            var failCount = 0;

            foreach (var filePath in validFiles)
            {
                try
                {
                    var fileInfo = new FileInfo(filePath);
                    Console.WriteLine();
                    Console.WriteLine($"Uploading: {fileInfo.Name}");

                    var progressBar = new ProgressBar(25);
                    progressBar.SetTotalBytes(fileInfo.Length);
                    var progress = new Progress<int>(percent => progressBar.Update(percent));

                    await s3Service.UploadFileAsync(filePath, progress);
                    progressBar.Complete();

                    ConsoleHelper.PrintSuccess($"Upload complete: {fileInfo.Name}");
                    
                    var url = await s3Service.GenerateDownloadUrlAsync(fileInfo.Name, settings.ShareLinkExpiryMinutes);
                    ConsoleHelper.PrintInfo($"URL: {url}");
                    
                    successCount++;
                }
                catch (Exception ex)
                {
                    ConsoleHelper.PrintError($"Upload failed for '{Path.GetFileName(filePath)}': {ex.Message}");
                    failCount++;
                }
            }

            Console.WriteLine();
            if (successCount > 0)
            {
                ConsoleHelper.PrintSuccess($"Successfully uploaded {successCount} file(s).");
            }
            if (failCount > 0)
            {
                ConsoleHelper.PrintError($"Failed to upload {failCount} file(s).");
            }

            ConsoleHelper.WaitForAnyKey();
        }

        private async Task DownloadFile()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.PrintHeader("DOWNLOAD FILE");

            var files = await s3Service.ListFilesAsync();

            if (files.Count == 0)
            {
                ConsoleHelper.PrintInfo("No files available to download.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            Console.WriteLine("Available files:");
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {files[i].FileName} ({files[i].Size})");
            }


            Console.WriteLine();
            Console.Write("Enter file number(s) to dowload (comma or space separated): ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                ConsoleHelper.PrintError("No file selected.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            var selectedFiles = new List<S3File>();


            var separators = new[] { ',', ' ', ';' };
            var parts = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                ConsoleHelper.PrintError("No file numbers entered.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            foreach (var part in parts)
            {
                var trimmedPart = part.Trim();
                if (string.IsNullOrWhiteSpace(trimmedPart))
                    continue;

                if (!int.TryParse(trimmedPart, out int fileIndex))
                {
                    ConsoleHelper.PrintError($"'{trimmedPart}' is not a valid number. Please enter numbers only (e.g., 1,2 or 1 2).");
                    ConsoleHelper.WaitForAnyKey();
                    return;
                }

                if (fileIndex < 1 || fileIndex > files.Count)
                {
                    ConsoleHelper.PrintError($"File number {fileIndex} is out of range. Please select between 1 and {files.Count}.");
                    ConsoleHelper.WaitForAnyKey();
                    return;
                }
                else
                {
                    selectedFiles.Add(files[fileIndex - 1]);

                }


            }

            if (selectedFiles.Count == 0)
            {
                ConsoleHelper.PrintError("No valid files selected.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            Console.Write($"Enter destination path (default: {settings.DefaultDownloadPath}): ");
            var destinationPath = Console.ReadLine()?.Trim();

            foreach (var selectedFile in selectedFiles)
            {

                if (string.IsNullOrEmpty(destinationPath))
                {
                    destinationPath = Path.Combine(settings.DefaultDownloadPath, selectedFile.FileName);
                }

                Console.WriteLine();
                Console.WriteLine("Downloading...");

                try
                {
                    await s3Service.DownloadFileAsync(selectedFile.S3Key, destinationPath);
                    ConsoleHelper.PrintSuccess($"Download complete!");
                    ConsoleHelper.PrintInfo($"Saved to: {destinationPath}");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.PrintError($"Download failed: {ex.Message}");
                }
            }

            ConsoleHelper.WaitForAnyKey();
        }

        private async Task DeleteFile()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.PrintHeader("DELETE FILE");

            var files = await s3Service.ListFilesAsync();

            if (files.Count == 0)
            {
                ConsoleHelper.PrintInfo("No files available to delete.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            Console.WriteLine("Available files:");
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {files[i].FileName} ({files[i].Size})");
            }

            Console.WriteLine();
            Console.Write("Enter file number(s) to delete (comma or space separated): ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                ConsoleHelper.PrintError("No file selected.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            var selectedFiles = new List<S3File>();
            
            
            var separators = new[] { ',', ' ', ';' };
            var parts = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                ConsoleHelper.PrintError("No file numbers entered.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            foreach (var part in parts)
            {
                var trimmedPart = part.Trim();
                if (string.IsNullOrWhiteSpace(trimmedPart))
                    continue;

                if (!int.TryParse(trimmedPart, out int fileIndex))
                {
                    ConsoleHelper.PrintError($"'{trimmedPart}' is not a valid number. Please enter numbers only (e.g., 1,2 or 1 2).");
                    ConsoleHelper.WaitForAnyKey();
                    return;
                }

                if (fileIndex < 1 || fileIndex > files.Count)
                {
                    ConsoleHelper.PrintError($"File number {fileIndex} is out of range. Please select between 1 and {files.Count}.");
                    ConsoleHelper.WaitForAnyKey();
                    return;
                }
                else
                {
                    selectedFiles.Add(files[fileIndex - 1]);

                }
                
                
            }

            if (selectedFiles.Count == 0)
            {
                ConsoleHelper.PrintError("No valid files selected.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Files to be deleted:");
            foreach (var file in selectedFiles)
            {
                Console.WriteLine($"  - {file.FileName} ({file.Size})");
            }

            Console.WriteLine();
            Console.Write($"Are you sure you want to delete {selectedFiles.Count} file(s)? (y/n): ");
            var confirm = Console.ReadLine()?.Trim().ToLower();

            if (confirm != "y" && confirm != "yes")
            {
                ConsoleHelper.PrintInfo("Deletion cancelled.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Deleting...");

            var successCount = 0;
            var failCount = 0;

            foreach (var file in selectedFiles)
            {
                try
                {
                    await s3Service.DeleteFileAsync(file.S3Key);
                    ConsoleHelper.PrintSuccess($"Deleted: {file.FileName}");
                    successCount++;
                }
                catch (Exception ex)
                {
                    ConsoleHelper.PrintError($"Failed to delete '{file.FileName}': {ex.Message}");
                    failCount++;
                }
            }

            Console.WriteLine();
            if (successCount > 0)
            {
                ConsoleHelper.PrintSuccess($"Successfully deleted {successCount} file(s).");
            }
            if (failCount > 0)
            {
                ConsoleHelper.PrintError($"Failed to delete {failCount} file(s).");
            }

            ConsoleHelper.WaitForAnyKey();
        }



        private async Task GenerateShareLink()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.PrintHeader("GENERATE SHARE LINK");

            var files = await s3Service.ListFilesAsync();

            if (files.Count == 0)
            {
                ConsoleHelper.PrintInfo("No files available.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            Console.WriteLine("Available files:");
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {files[i].FileName} ({files[i].Size})");
            }

            Console.WriteLine();
            Console.Write("Enter file number(s) to dowload (comma or space separated): ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                ConsoleHelper.PrintError("No file selected.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            var selectedFiles = new List<S3File>();

            var separators = new[] { ',', ' ', ';' };
            var parts = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                ConsoleHelper.PrintError("No file numbers entered.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }

            foreach (var part in parts)
            {
                var trimmedPart = part.Trim();
                if (string.IsNullOrWhiteSpace(trimmedPart))
                    continue;

                if (!int.TryParse(trimmedPart, out int fileIndex))
                {
                    ConsoleHelper.PrintError($"'{trimmedPart}' is not a valid number. Please enter numbers only (e.g., 1,2 or 1 2).");
                    ConsoleHelper.WaitForAnyKey();
                    return;
                }

                if (fileIndex < 1 || fileIndex > files.Count)
                {
                    ConsoleHelper.PrintError($"File number {fileIndex} is out of range. Please select between 1 and {files.Count}.");
                    ConsoleHelper.WaitForAnyKey();
                    return;
                }
                else
                {
                    selectedFiles.Add(files[fileIndex - 1]);

                }


            }

            if (selectedFiles.Count == 0)
            {
                ConsoleHelper.PrintError("No valid files selected.");
                ConsoleHelper.WaitForAnyKey();
                return;
            }
            
            Console.Write($"Enter expiry time in minutes (default: {settings.ShareLinkExpiryMinutes}): ");
            var expiryInput = Console.ReadLine()?.Trim();
            int expiryMinutes = settings.ShareLinkExpiryMinutes;

            if (!string.IsNullOrEmpty(expiryInput) && int.TryParse(expiryInput, out int parsedExpiry))
            {
                expiryMinutes = parsedExpiry;
            }

            Console.WriteLine();
            Console.WriteLine("Generating share link...");

            foreach (var selectedFile in selectedFiles)
            {
                try
                {
                    var url = await s3Service.GenerateDownloadUrlAsync(selectedFile.S3Key, expiryMinutes);
                    Console.WriteLine();
                    ConsoleHelper.PrintSuccess("Share link generated!");
                    ConsoleHelper.PrintInfo($"File: {selectedFile.FileName}");
                    ConsoleHelper.PrintInfo($"Expires in: {expiryMinutes} minutes");
                    ConsoleHelper.PrintInfo($"URL: {url}");
                }
                catch (Exception ex)
                {
                    ConsoleHelper.PrintError($"Failed to generate link: {ex.Message}");
                }

            }

            ConsoleHelper.WaitForAnyKey();
        }

        private void ShowSettings()
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.PrintHeader("SETTINGS");

            ConsoleHelper.PrintInfo($"Max File Size: {fileValidator.FormatFileSize(settings.MaxFileSizeBytes)}");
            ConsoleHelper.PrintInfo($"Allowed File Types: {string.Join(", ", settings.AllowedFileTypes)}");
            ConsoleHelper.PrintInfo($"Share Link Expiry: {settings.ShareLinkExpiryMinutes} minutes");
            ConsoleHelper.PrintInfo($"Default Download Path: {settings.DefaultDownloadPath}");

            ConsoleHelper.WaitForAnyKey();
        }
    }
}

