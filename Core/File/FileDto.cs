using DataLayer.Base;

namespace Parsia.Core.File
{
    public class FileDto : BaseDto
    {
        public long EntityId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Alt { get; set; }
        public string DataSize { get; set; }
        public static string DataType { get; set; }
        public string Description { get; set; }
        public string Extension { get; set; }
        public long? ParentId { get; set; }
        public string Thumbnail { get; set; }
        public bool DisplayInFileManager { get; set; }
    }
}