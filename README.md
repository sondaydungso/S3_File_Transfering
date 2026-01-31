# S3 File Manager - Console Edition

A simple command-line tool for managing files in your AWS S3 bucket. Upload, download, delete, and share files directly from your terminal.

## What This Does

This application gives you a menu-driven interface to work with files stored in Amazon S3. You can list all your files, upload new ones, download existing files, delete files, and generate shareable links. Everything runs in the console, so it's lightweight and easy to use.

## Prerequisites

Before you can use this tool, you'll need:

- .NET 8.0 SDK installed on your computer
- An AWS account with S3 access
- An S3 bucket set up
- AWS access credentials (Access Key ID and Secret Access Key)

## Setup

### 1. Clone or Download the Project

Get the project files onto your computer. If you're using Git, clone the repository. Otherwise, just download the files.

### 2. Create Your Environment File

Create a file named `.env` in the project root directory. This file will store your AWS credentials. Here's what it should look like:

```
Access key ID=your_access_key_id_here
Secret access key=your_secret_access_key_here
S3_BUCKET_NAME=your_bucket_name_here
AWS_REGION=us-east-1
```

Replace the placeholder values with your actual AWS credentials:
- Get your Access Key ID and Secret Access Key from the AWS IAM console
- Use the name of the S3 bucket you want to work with
- Set the region where your bucket is located (like us-east-1, eu-west-1, etc.)

Important: Never commit your `.env` file to version control. It contains sensitive information.

### 3. Build the Project

Open a terminal in the project directory and run:

```
dotnet build
```

This will compile the application and download any required packages.

### 4. Run the Application

Once built, you can run it with:

```
dotnet run
```

Or if you want to run the compiled executable directly:

```
dotnet run --no-build
```

## How to Use

When you start the application, you'll see a menu with seven options:

1. **List Files** - Shows all files currently in your S3 bucket, along with their sizes, types, and when they were last modified.

2. **Upload File** - Upload a file from your computer to S3. You'll be asked for the file path (you can drag and drop the file into the terminal). The app will validate the file, show upload progress, and give you a shareable link when done.

3. **Download File** - Download a file from S3 to your computer. Select the file from a numbered list, then choose where to save it.

4. **Delete File** - Remove files from your S3 bucket. You can delete multiple files at once by entering their numbers separated by commas (like 1,2,3).

5. **Generate Share Link** - Create a temporary link to share a file. You can set how long the link should be valid (default is 60 minutes).

6. **Settings** - View your current configuration settings like maximum file size, allowed file types, and default download location.

7. **Exit** - Close the application.

## File Validation

The app checks files before uploading to make sure they meet certain criteria:

- The file must exist on your computer
- The file size must be within the limit (default is 100 MB)
- The file type must be in the allowed list (PDF, DOC, images, etc.)

You can modify these settings in the `Models/AppSettings.cs` file if needed.

## Configuration

Default settings are defined in `Models/AppSettings.cs`. You can change:

- Maximum file size allowed for uploads
- Which file types are permitted
- How long share links remain valid
- Where downloaded files are saved by default

## Troubleshooting

If you get an error about AWS credentials not being found, make sure:

- Your `.env` file exists in the project root
- The file is named exactly `.env` (with the dot at the beginning)
- All four required variables are set with the correct values
- There are no extra spaces around the equals signs

If uploads or downloads fail, check:

- Your internet connection
- That your AWS credentials have the necessary permissions
- That your S3 bucket name and region are correct
- That the file path you're using is valid

## Project Structure

The code is organized into a few main folders:

- **Models** - Data structures like file information and settings
- **Services** - Core functionality for S3 operations and file validation
- **UI** - User interface components like menus and progress bars

This keeps things organized and makes it easier to understand and modify the code.

## Notes

This tool is designed for personal use or small teams. For production environments with many users, you might want additional features like user authentication, logging, or more sophisticated error handling.

The application uses the AWS SDK for .NET to communicate with S3. Make sure your AWS credentials have the necessary permissions to read, write, and delete objects in your bucket.

## License

This project is provided as-is for your use. Modify it as needed for your purposes.

## Version

The current version of this application is 1.0.0. This is the first release and may receive updates in the future, If you have suggestions or find issues, feel free to contribute!
Current version only supports ".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png", although all the files extension are listed in the settings and can be modified in the code, feel free to try it out on your local

