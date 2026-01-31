using S3FileManager.Models;
using S3FileManager.Services;
using S3FileManager.UI;
using System.Threading.Tasks;

namespace S3FileManager
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var settings = new AppSettings();
                var s3Service = new S3Service();
                var fileValidator = new FileValidator(settings);
                var menuManager = new MenuManager(s3Service, fileValidator, settings);

                while (true)
                {
                    menuManager.ShowMainMenu();
                    
                    var input = Console.ReadLine();
                    if (int.TryParse(input, out int choice))
                    {
                        await menuManager.HandleMenuChoice(choice);
                    }
                    else
                    {
                        ConsoleHelper.PrintError("Invalid input. Please enter a number between 1-7.");
                        ConsoleHelper.WaitForAnyKey();
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.ClearScreen();
                ConsoleHelper.PrintHeader("CONFIGURATION ERROR");
                ConsoleHelper.PrintError(ex.Message);
                Console.WriteLine();
                ConsoleHelper.PrintInfo("To fix this issue:");
                ConsoleHelper.PrintInfo("1. Create a .env file in the application directory");
                ConsoleHelper.PrintInfo("2. Add the following variables:");
                Console.WriteLine();
                Console.WriteLine("   Access key ID=your_access_key_id");
                Console.WriteLine("   Secret access key=your_secret_access_key");
                Console.WriteLine("   S3_BUCKET_NAME=your_bucket_name");
                Console.WriteLine("   AWS_REGION=us-east-1");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
