using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3FileUploader.Model
{
    public class S3File
    {
        private string _fileName;
        private string _fileType;
        private long _fileSize;
        private DateTime _lastModified;
        private DateTime _uploadAt;
        private string _size;
        private string _s3key;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }
        public DateTime LastModified
        {
            get { return _lastModified; }
            set { _lastModified = value; }
        }
        public DateTime UploadAt
        {
            get { return _uploadAt; }
            set { _uploadAt = value; }
        }
        public string Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public string S3Key
        {
            get { return _s3key; }
            set { _s3key = value; }
        }
        public string FileType
        {
            get { return _fileType; }
            set { _fileType = value; }
        }

    }
}
