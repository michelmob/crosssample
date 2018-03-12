using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gravity.Diagnostics;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Service;
using Gravity.Manager.Web.Application;
using Gravity.Manager.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace Gravity.Manager.Web.Controllers
{
    [Authorize]
    public class MembersController : BaseMvcController
    {
        private readonly ILogger _logger;
        private readonly IMemberService _memberService;
        private readonly IUserStateWrapper _userStateWrapper;

        public MembersController(ILogger logger
            , IMemberService memberService
            , IUserStateWrapper userStateWrapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _userStateWrapper = userStateWrapper ?? throw new ArgumentNullException(nameof(userStateWrapper));
        }
        
        public async Task<IActionResult> Index()
        {
            // We might but it in BaseMvcController constructor.
            CheckAndSetErrorMessage();
            
            var memberlist = await _memberService.GetMemberListAsync();
            
            return View(memberlist);
        }
        
        public async Task<IActionResult> Edit(long id)
        {
            if (id <= 0)
            {
                SetTempDataErrorMessage("You didn't select a valid record to edit. Please select an existing record to edit.");
                return RedirectToAction("Index");
            }
            
            var user = await _memberService.GetUserByIdAsync(id);
            var model = new UserEditViewModel(user);
            var organizations = await _memberService.GetAllOrganizationsAsync();
            model.Organizations = organizations;
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            var organizations = await _memberService.GetAllOrganizationsAsync();
            model.Organizations = organizations;
            if (!ModelState.IsValid)
            {    
                return View(model);
            }
            
            var user = await _memberService.GetUserByIdAsync(model.Id);
            user = model.ChangeUser(user);
            // TODO: Check result
            var result = await _memberService.UpdateUserAsync(user);
            // user = await _memberService.GetUserByIdAsync(model.Id);
            //model.FromUser(user);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
            {
                SetTempDataErrorMessage("You didn't select a valid record to delete. Please select an existing record to delete.");
                return RedirectToAction("Index");
            }
            
            var userState = _userStateWrapper.GetUserState();
            if (id == userState.Id)
            {
                SetTempDataErrorMessage("You can not delete yourself.");
                return RedirectToAction("Index");
            }

            await _memberService.DeleteMemberAsync(id);
            return RedirectToAction("Index");
        }
    }
}
