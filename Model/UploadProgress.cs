using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3FileUploader.Model
{
    public class UploadProgress
    {
        private List<S3File> _numberOfFile;
        private long _totalBytes;
        private long _uploadedBytes;
        private int _percentCompleted;
        private List<string> _status;
        private string? _errorMessage;
        public long TotalBytes
        {
            get { return _totalBytes; }
            set { _totalBytes = value; }
        }
        public long UploadedBytes
        {
            get { return _uploadedBytes; }
            set { _uploadedBytes = value; }
        }
        public int PercentCompleted
        {
            get { return _percentCompleted; }
            set { _percentCompleted = value; }
        }
        public List<string> Status
        {
            get { return _status; }
            set { _status = value; }
        }
        public string? ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }
        public List<S3File> NumberOfFile
        {
            get { return _numberOfFile; }
            set { _numberOfFile = value; }
        }
    }
}
