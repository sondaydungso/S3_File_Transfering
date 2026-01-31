using System;

namespace S3FileManager.Services
{
    public class AppCredentials
    {
        private static bool envLoaded = false;

        private void EnsureEnvLoaded()
        {
            if (!envLoaded)
            {
                envLoaded = EnvParser.Load(".env");
            }
        }

        public string GetAccessKeyId()
        {
            EnsureEnvLoaded();
            return Environment.GetEnvironmentVariable("Access key ID");
        }

        public string GetSecretAccessKey()
        {
            EnsureEnvLoaded();
            return Environment.GetEnvironmentVariable("Secret access key");
        }

        public string BucketName()
        {
            EnsureEnvLoaded();
            return Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
        }

        public string Region()
        {
            EnsureEnvLoaded();
            return Environment.GetEnvironmentVariable("AWS_REGION");
        }
    }
}

