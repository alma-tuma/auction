using Auction.Entity;
using Auction.Entity.Enumerations;
using Auction.Infrastructure.EF;
using Auction.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Auction.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationContext context, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        public IActionResult LoginOrRegister()
        {
            var loginModel = JsonConvert.DeserializeObject<LoginModel>(GetTempData("LoginModel"));
            var registerModel = JsonConvert.DeserializeObject<RegisterModel>(GetTempData("RegisterModel"));
            var errors = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(GetTempData("Errors"));

            if (errors != null)
            {
                foreach (var error in errors)
                {
                    var errerMessage = string.Join(",", error.Value);
                    ModelState.AddModelError(error.Key, errerMessage);
                }
            }

            return View(new LoginOrRegisterModel()
            {
                LoginModel = loginModel,
                RegisterModel = registerModel
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToList()
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage).ToList());

                SetTempData("LoginModel", loginModel);
                SetTempData("Errors", errors);

                return RedirectToAction(nameof(LoginOrRegister));
            }
            var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, false);
            if (!result.Succeeded)
            {
                var errors = new Dictionary<string, List<string>>();
                errors.Add("LoginModel.Password", new List<string>() { "Username or passord are not correct" });

                SetTempData("LoginModel", loginModel);
                SetTempData("Errors", errors);
                return RedirectToAction(nameof(LoginOrRegister));
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToList()
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage).ToList());

                SetTempData("RegisterModel", registerModel);
                SetTempData("Errors", errors);

                return RedirectToAction(nameof(LoginOrRegister));
            }

            var user = new ApplicationUser
            {
                UserName = registerModel.Username,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                var errorMessage = string.Join(",", result.Errors.Select(x => x.Description).ToList());
                var errors = new Dictionary<string, List<string>>();
                errors.Add("RegisterModel.Username", new List<string>() { errorMessage });

                SetTempData("RegisterModel", registerModel);
                SetTempData("Errors", errors);

                return RedirectToAction(nameof(LoginOrRegister));
            }

            var transaction = new Transaction
            {
                UserId = user.Id,
                TransactionType = TransactionTypeEnum.Deposit,
                Amount = 1000
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            var signInResult = await _signInManager.PasswordSignInAsync(registerModel.Username, registerModel.Password, false, false);
            if (!signInResult.Succeeded)
                return RedirectToAction(nameof(LoginOrRegister));

            return RedirectToAction("Index", "Home");
        }

        private void SetTempData(string key, object value)
        {
            TempData[key] = JsonConvert.SerializeObject(value);
        }

        private string GetTempData(string key)
        {
            if (TempData[key] == null)
                return string.Empty;

            return TempData[key].ToString();
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("LoginOrRegister", "Account");
        }
    }
}
