using Filer.Api.Shared;
using Filer.Services;

namespace Filer.Pages.Shared
{
    public class FolderBaseController : BaseController
    {
        protected readonly FolderService _folderService;
        public FolderBaseController(
            IConfiguration configuration)
            : base(configuration)
        {
            _folderService = new FolderService(
                _useHistory,
                _useWindowsNaturalSort,
                _useVariantSearch,
                _workDirs);
        }
    }
}