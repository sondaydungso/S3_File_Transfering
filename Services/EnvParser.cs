using System.IO;

namespace S3FileManager.Services
{
    public class EnvParser
    {
        public static bool Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();
                Environment.SetEnvironmentVariable(key, value);
            }

            return true;
        }
    }
}

