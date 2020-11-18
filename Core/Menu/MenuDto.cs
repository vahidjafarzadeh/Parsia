using DataLayer.Base;
using Parsia.Core.ComboVal;
using Parsia.Core.File;
using Parsia.Core.UseCase;

namespace Parsia.Core.Menu
{
    public class MenuDto : BaseDto
    {
        public long EntityId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public int OrderNode { get; set; }
        public FileDto File { get; set; }
        public UseCaseDto UseCase { get; set; }
        public MenuDto Parent { get; set; }
        public ComboValDto Target { get; set; }
    }
}