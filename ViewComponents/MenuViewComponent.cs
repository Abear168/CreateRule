using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CreateRule.Data;
using CreateRule.Models;
using CreateRule.Services;

namespace CreateRule.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;
        private readonly UserManager<ApplicationUser> _userManager;

        public MenuViewComponent(IMenuService menuService, UserManager<ApplicationUser> userManager)
        {
            _menuService = menuService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Content(string.Empty);
            }

            var menuItems = await _menuService.GetMenuItemsForUserAsync(user.Id);
            return View(menuItems);
        }
    }
}
