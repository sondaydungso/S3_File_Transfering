using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3_File_Transfering.Model
{
    public class AppSetting
    {
        
        public long MaxFileSizeBytes { get; set; } = 100 * 1024 * 1024;
        public List<string> AllowedFileTypes { get; set; } = new List<string>
        {
            ".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png"
        };
        public int ShareLinkExpiryMinutes { get; set; } = 60;
        public string DefaultDownloadPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }
}

