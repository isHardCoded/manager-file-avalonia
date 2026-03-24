using System;

namespace fileManager.Models
{
    public class FileItem
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public long Size { get; set; }
        public DateTime Created { get; set; }
        public bool IsDirectory { get; set; }
    }
}
