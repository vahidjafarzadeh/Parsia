using DataLayer.Base;

namespace Parsia.Core.File
{
    public class FolderDto : BaseDto
    {
        public string FolderName { get; set; }
        public string Path { get; set; }
        public long? ParentId { get; set; }
    }
}