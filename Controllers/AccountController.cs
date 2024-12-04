using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Online.Areas.Admin.Repository;
using Shopping_Online.Data;
using Shopping_Online.Models;
using Shopping_Online.Models.ViewModels;

namespace Shopping_Online.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly DataContext _dataContext;

        public AccountController(UserManager<AppUserModel> userManager,
                                SignInManager<AppUserModel> signInManager,
                                IEmailSender emailSender,
                                DataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _dataContext = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["HideSlider"] = true;
            ViewData["HideSearchBox"] = true;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        public async Task<IActionResult> UpdateAccount()
        {
            ViewData["HideSlider"] = true;
            ViewData["HideSearchBox"] = true;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateInfoAccount(AppUserModel user)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userById = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userById == null)
            {
                return NotFound();
            }

            // Cập nhật số điện thoại nếu thay đổi
            if (!string.IsNullOrEmpty(user.PhoneNumber) && user.PhoneNumber != userById.PhoneNumber)
            {
                userById.PhoneNumber = user.PhoneNumber;
            }

            _dataContext.Update(userById);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Cập nhật thông tin thành công";

            return RedirectToAction("UpdateAccount", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userById = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userById == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu không nhập mật khẩu cũ
            if (string.IsNullOrEmpty(currentPassword))
            {
                TempData["error"] = "Bạn cần nhập mật khẩu cũ để thay đổi mật khẩu.";
                return RedirectToAction("UpdateAccount", "Account");
            }

            // Kiểm tra mật khẩu cũ
            var passwordHasher = new PasswordHasher<AppUserModel>();
            var verifyResult = passwordHasher.VerifyHashedPassword(userById, userById.PasswordHash, currentPassword);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                TempData["error"] = "Mật khẩu cũ không chính xác.";
                return RedirectToAction("UpdateAccount", "Account");
            }

            // Cập nhật mật khẩu mới
            if (!string.IsNullOrEmpty(newPassword))
            {
                userById.PasswordHash = passwordHasher.HashPassword(userById, newPassword);

                _dataContext.Update(userById);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thay đổi mật khẩu thành công.";
            }

            return RedirectToAction("UpdateAccount", "Account");
        }
        public async Task<IActionResult> NewPass(AppUserModel user, string token)
        {
            var checkuser = await _userManager.Users
                            .Where(u => u.Email == user.Email)
                            .Where(u => u.Token == user.Token)
                            .FirstOrDefaultAsync();

            if (checkuser != null)
            {
                ViewBag.Email = checkuser.Email;
                ViewBag.Token = token;
            }
            else
            {
                TempData["error"] = "Không thấy người dùng";
                return RedirectToAction("ForgetPass", "Account");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNewPass(AppUserModel user, string token)
        {
            var checkuser = await _userManager.Users
                            .Where(u => u.Email == user.Email)
                            .Where(u => u.Token == user.Token)
                            .FirstOrDefaultAsync();

            if (checkuser != null)
            {
                string newtoken = Guid.NewGuid().ToString();
                var passwordHasher = new PasswordHasher<AppUserModel>();
                var hashedPassword = passwordHasher.HashPassword(checkuser, user.PasswordHash);

                checkuser.PasswordHash = hashedPassword;
                checkuser.Token = newtoken;

                await _userManager.UpdateAsync(checkuser);
                TempData["success"] = "Đổi mật khẩu thành công";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["error"] = "Không thấy người dùng";
                return RedirectToAction("ForgetPass", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMailForgetPass(AppUserModel user)
        {
            var checkMail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (checkMail == null)
            {
                TempData["error"] = "Không thấy Email";
                return RedirectToAction("ForgetPass", "Account");
            }
            else
            {
                string token = Guid.NewGuid().ToString();
                checkMail.Token = token;
                _dataContext.Update(checkMail);
                await _dataContext.SaveChangesAsync();
                var receiver = checkMail.Email;
                var subject = "Thay đổi mật khẩu của người dùng " + checkMail.Email;
                var message = "Vui lòng nhấn vào link sau để thay đổi mật khẩu: " +
                "<a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email=" + checkMail.Email + "&token=" + token + "'>";

                await _emailSender.SendEmailAsync(receiver, subject, message);
            }

            TempData["success"] = "Vui lòng kiểm tra Email để thay đổi mật khẩu";
            return RedirectToAction("ForgetPass", "Account");
        }
        public async Task<IActionResult> ForgetPass(string returnUrl)
        {
            ViewData["HideSlider"] = true;
            ViewData["HideSearchBox"] = true;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(loginVM.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu");
            }
            return View(loginVM);
        }
        public IActionResult Create()
        {
            ViewData["HideSlider"] = true;
            ViewData["HideSearchBox"] = true;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                AppUserModel newUser = new AppUserModel()
                {
                    UserName = user.UserName,
                    Email = user.Email
                };
                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {
                    TempData["success"] = "Đăng ký thành công";
                    return Redirect("/account/login");
                }
            }
            return View(user);
        }

        public async Task<IActionResult> History()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var Orders = await _dataContext.Orders
                .Where(od => od.UserName == userEmail)
                .OrderByDescending(od => od.Id).ToListAsync();
            ViewBag.userEmail = userEmail;
            return View(Orders);
        }

        public async Task<IActionResult> CancelOrder(string orderCode)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                var order = await _dataContext.Orders
                    .Where(o => o.OrderCode == orderCode).FirstAsync();
                order.Status = 3;
                _dataContext.Update(order);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("error");
            }
            return RedirectToAction("History", "Account");
        }

        // [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

    }
}