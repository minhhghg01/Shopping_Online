using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Data;
using Shopping_Online.Models;

namespace Shopping_Online.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _userManager.Users.OrderByDescending(p => p.Id).ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (createUserResult.Succeeded)
                {
                    TempData["success"] = "Tạo tài khoản thành công";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["error"] = "Lỗi";
                return BadRequest();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AppUserModel user)
        {
            var userEdit = await _userManager.FindByIdAsync(id);
            if (userEdit == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                userEdit.UserName = user.UserName;
                userEdit.Email = user.Email;
                userEdit.PhoneNumber = user.PhoneNumber;
                userEdit.RoleId = user.RoleId;

                var result = await _userManager.UpdateAsync(userEdit);
                if (result.Succeeded)
                {
                    TempData["success"] = "Cập nhật tài khoản thành công";
                    return RedirectToAction("Index");
                }
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "Xóa tài khoản thành công";
            return RedirectToAction("Index");
        }
    }
}