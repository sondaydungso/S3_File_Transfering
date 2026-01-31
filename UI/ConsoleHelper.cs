using System;

namespace S3FileManager.UI
{
    public static class ConsoleHelper
    {
        public static void PrintHeader(string title)
        {
            var width = 47;
            var border = new string('═', width);
            var padding = (width - title.Length) / 2;
            var paddedTitle = title.PadLeft(title.Length + padding);

            Console.WriteLine();
            Console.WriteLine(border);
            Console.WriteLine(paddedTitle);
            Console.WriteLine(border);
            Console.WriteLine();
        }

        public static void PrintSuccess(string message)
        {
            Console.WriteLine($"✓ {message}");
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            Console.ResetColor();
        }

        public static void PrintInfo(string message)
        {
            Console.WriteLine($"  {message}");
        }

        public static void WaitForAnyKey()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to return to menu...");
            Console.ReadKey(true);
        }

        public static void ClearScreen()
        {
            Console.Clear();
        }
    }
}

