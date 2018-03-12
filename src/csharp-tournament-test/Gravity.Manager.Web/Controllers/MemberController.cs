using System;
using System.Threading.Tasks;
using Gravity.Diagnostics;
using Gravity.Manager.ApplicationService;
using Gravity.Manager.Service;
using Gravity.Manager.Web.Application;
using Gravity.Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gravity.Manager.Web.Controllers
{
    [Authorize]
    public class MemberController : BaseMvcController
    {
        private readonly ILogger _logger;
        private readonly ISigninManager _signinManager;
        private readonly IExternalAuthenticationProvider _externalAuthenticationProvider;
        private readonly IMemberAppService _memberService;
        private readonly IUserStateWrapper _userStateWrapper;

        public const string InvalidLogonMessage = "Invalid user name or password.";
        public const string RegisteringNewUserFailureMessage = "Couldn't register new user plase try again.";
        public const string NotMachingUserId = "You can only update your profile.";
        

        public MemberController(ILogger logger
            , ISigninManager signinManager
            , IExternalAuthenticationProvider externalAuthenticationProvider
            , IMemberAppService memberService
            , IUserStateWrapper userStateWrapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signinManager = signinManager ?? throw new ArgumentNullException(nameof(signinManager));
            _externalAuthenticationProvider = externalAuthenticationProvider ?? throw new ArgumentNullException(nameof(externalAuthenticationProvider));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _userStateWrapper = userStateWrapper ?? throw new ArgumentNullException(nameof(userStateWrapper));
        }
        
        // TODO: Use captcha
        [AllowAnonymous]
        public IActionResult Logon()
        {
            var userState = _userStateWrapper.GetUserState();
            if (userState == null || userState.Id == 0)
            {
                return View();                
            }
            if (string.IsNullOrWhiteSpace(userState.Name) || string.IsNullOrWhiteSpace(userState.EMail))
            {
                // Not registered properly before
                return RedirectToAction("Index", "Member");
            }
            // No need to logon or register again
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logon(LogonViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Authenticate first
                var authenticated = _externalAuthenticationProvider.Authenticate(model.UserName, model.Password);

                if (authenticated)
                {
                    var user = await _memberService.GetUserByUsernameAsync(model.UserName);
                    if (user != null)
                    {
                        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.EMail))
                        {
                            // Not Registered properly
                            await _signinManager.SignInAsync(UserState.FromUser(user));
                            return RedirectToAction("Index", "Member");
                        }
                        // Registered properly
                        await _signinManager.SignInAsync(UserState.FromUser(user));
                        return RedirectToAction("Index", "Home");
                    }
                    var result = await _memberService.RegisterNewUserAsync(model.UserName);
                    if (result == 1)
                    {
                        user = _memberService.GetUserByUsernameAsync(model.UserName).GetAwaiter().GetResult();
                        await _signinManager.SignInAsync(UserState.FromUser(user));
                        return RedirectToAction("Index", "Member");
                    }
                    // Could't register, might try again.
                    ModelState.AddModelError(Constants.ModelStateCustomErrorKey, RegisteringNewUserFailureMessage);
                }
                else
                {
                    _logger.Warn($"Authentication failure for user {model.UserName}!");
                    ModelState.AddModelError(Constants.ModelStateCustomErrorKey, InvalidLogonMessage);
                }
            }
            model = new LogonViewModel
            {
                UserName = model.UserName
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signinManager.SignOffAsync();
            return RedirectToAction("Logon", "Member");
        }

        
        public async Task<IActionResult> Index()
        {
            var userState = _userStateWrapper.GetUserState();
            if (userState == null || userState.Id == 0)
            {
                return RedirectToAction("Logon", "Member");
            }
            var user = await _memberService.GetUserByIdAsync(userState.Id);
            if (user != null)
            {
                var model = new UserProfileViewModel();
                model.FromUser(user);
                return View(model);
            }
            return RedirectToAction("Logon", "Member");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserProfileViewModel model)
        {
            var userState = _userStateWrapper.GetUserState();
            if (userState == null || userState.Id == 0)
            {
                return RedirectToAction("Logon", "Member");
            }
            
            if (ModelState.IsValid)
            {
                if (model.Id != userState.Id)
                {
                    ModelState.AddModelError(Constants.ModelStateCustomErrorKey, NotMachingUserId);
                }
                else
                {
                    var user = await _memberService.GetUserByIdAsync(userState.Id);
                    
                    user = model.ChangeUser(user);
                    // TODO: Check result
                    var result = await _memberService.UpdateUserAsync(user);
                    await _signinManager.SignOffAsync();
                    await _signinManager.SignInAsync(UserState.FromUser(user));
                    model.FromUser(user);                    
                }
            }
            return View(model);
        }
    }
}