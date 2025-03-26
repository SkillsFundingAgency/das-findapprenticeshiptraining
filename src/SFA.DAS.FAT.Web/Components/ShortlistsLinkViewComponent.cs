using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Web.Models;
using SFA.DAS.FAT.Web.Models.Shared.Components;

namespace SFA.DAS.FAT.Web.Components;

public class ShortlistsLinkViewComponent(ICookieStorageService<ShortlistCookieItem> _shortlistCookieService, IShortlistService _shortlistService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var cookie = _shortlistCookieService.Get(Constants.ShortlistCookieName);

        var shortlistsCount = 0;

        if (cookie != null)
        {
            shortlistsCount = await _shortlistService.GetShortlistsCountForUser(cookie.ShortlistUserId);
        }

        return View(new ShortlistsLinkComponentViewModel
        {
            Count = shortlistsCount
        });
    }
}
