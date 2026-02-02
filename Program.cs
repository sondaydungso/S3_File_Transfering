using S3_File_Transfering.Services;
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
                
                string userId = null;
                bool isLoggedIn = false;
                string username = null;

                while (!isLoggedIn)
                {
                    ConsoleHelper.ClearScreen();
                    Console.WriteLine("═══════════════════════════════════════════");
                    Console.WriteLine("    S3 FILE MANAGER - Welcome");
                    Console.WriteLine("═══════════════════════════════════════════");
                    Console.WriteLine();
                    Console.WriteLine("1. Login");
                    Console.WriteLine("2. Register");
                    Console.WriteLine("3. Exit");
                    Console.WriteLine();
                    Console.Write("Enter choice (1-3): ");
                    
                    var choice = Console.ReadLine()?.Trim();

                    if (choice == "1")
                    {
                        // Login
                        ConsoleHelper.ClearScreen();
                        ConsoleHelper.PrintHeader("LOGIN");
                        Console.WriteLine();

                        Console.Write("Username: ");
                        username = Console.ReadLine()?.Trim();

                        if (string.IsNullOrEmpty(username))
                        {
                            ConsoleHelper.PrintError("Username cannot be empty.");
                            ConsoleHelper.WaitForAnyKey();
                            continue;
                        }

                        Console.Write("Password: ");
                        string password = Console.ReadLine()?.Trim();

                        if (string.IsNullOrEmpty(password))
                        {
                            ConsoleHelper.PrintError("Password cannot be empty.");
                            ConsoleHelper.WaitForAnyKey();
                            continue;
                        }

                        Console.WriteLine();
                        Console.WriteLine("Validating credentials...");

                        if (Authentication.ValidateUser(username, password))
                        {
                            
                            userId = Authentication.GetUserId(username).ToString();
                            isLoggedIn = true;
                            ConsoleHelper.PrintSuccess("Login successful!");
                            Console.WriteLine();
                            ConsoleHelper.PrintInfo($"Welcome, {username}!");
                            await Task.Delay(1500); 
                        }
                        else
                        {
                            ConsoleHelper.PrintError("Invalid username or password. Please try again.");
                            ConsoleHelper.WaitForAnyKey();
                        }
                    }
                    //register
                    else if (choice == "2")
                    {
                        
                        ConsoleHelper.ClearScreen();
                        ConsoleHelper.PrintHeader("REGISTER");
                        Console.WriteLine();

                        Console.Write("Username: ");
                        string newUsername = Console.ReadLine()?.Trim();

                        if (string.IsNullOrEmpty(newUsername))
                        {
                            ConsoleHelper.PrintError("Username cannot be empty.");
                            ConsoleHelper.WaitForAnyKey();
                            continue;
                        }

                        
                        if (Authentication.UsernameExists(newUsername))
                        {
                            ConsoleHelper.PrintError("Username already exists. Please choose a different username.");
                            ConsoleHelper.WaitForAnyKey();
                            continue;
                        }

                        Console.Write("Password: ");
                        string newPassword = Console.ReadLine()?.Trim();

                        if (string.IsNullOrEmpty(newPassword))
                        {
                            ConsoleHelper.PrintError("Password cannot be empty.");
                            ConsoleHelper.WaitForAnyKey();
                            continue;
                        }

                        Console.Write("Confirm Password: ");
                        string confirmPassword = Console.ReadLine()?.Trim();

                        if (newPassword != confirmPassword)
                        {
                            ConsoleHelper.PrintError("Passwords do not match.");
                            ConsoleHelper.WaitForAnyKey();
                            continue;
                        }

                        Console.WriteLine();
                        Console.WriteLine("Registering user...");

                        
                        if (Authentication.Register(newUsername, newPassword))
                        {
                            ConsoleHelper.PrintSuccess("Registration successful!");
                            Console.WriteLine();
                            ConsoleHelper.PrintInfo("You can now login with your credentials.");
                            ConsoleHelper.WaitForAnyKey();
                        }
                        else
                        {
                            ConsoleHelper.PrintError("Registration failed. Please try again.");
                            ConsoleHelper.WaitForAnyKey();
                        }
                    }
                    //bye - bye
                    else if (choice == "3")
                    {
                        Console.WriteLine("Goodbye!");
                        Environment.Exit(0);
                    }
                    else
                    {
                        ConsoleHelper.PrintError("Invalid choice. Please select 1-3.");
                        ConsoleHelper.WaitForAnyKey();
                    }
                }

                
                var settings = new AppSettings();
                var s3Service = new S3Service(userId); 
                var fileValidator = new FileValidator(settings);
                var menuManager = new MenuManager(userId, s3Service, fileValidator, settings); 

                
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
                Console.WriteLine("   Server=your server ip");
                Console.WriteLine("   Database= The database for user-loggin");
                Console.WriteLine("   User_ID= the database user");
                Console.WriteLine("   Password= your user password");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
