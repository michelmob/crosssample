using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Service;
using Gravity.Manager.Service.Ldap;
using Gravity.Manager.Web.Application;
using Gravity.Manager.Web.Controllers;
using Gravity.Manager.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Gravity.Manager.Web.Tests.Controllers
{
    [TestFixture]
    public class MemberControllerTests
    {
        private TestLogger _testLogger;
        private Mock<ISigninManager> _signinManagerMock;
        private Mock<IExternalAuthenticationProvider> _ldapAuthenticationProviderMock;
        private Mock<IMemberService> _memberServiceMock;
        private Mock<IUserStateWrapper> _userStateWrapperNullMock;
        private Mock<IUserStateWrapper> _userStateWrapperMock;
        private Mock<IUserStateWrapper> _userStateWrapperNotRegisteredMock;
        private MemberController _controllerWitNullUserState;
        private MemberController _controllerWithUserState;
        private MemberController _controllerWithNotRegisteredUserState;


        [SetUp]
        public void TestSetUp()
        {
            _testLogger = new TestLogger();
            _signinManagerMock = new Mock<ISigninManager>();
            _ldapAuthenticationProviderMock = new Mock<IExternalAuthenticationProvider>();
            _ldapAuthenticationProviderMock.Setup(m => m.Authenticate(It.Is<string>(s => s == "admin"), It.IsAny<string>())).Returns(true);
            _ldapAuthenticationProviderMock.Setup(m => m.Authenticate(It.Is<string>(s => s == "admin2"), It.IsAny<string>())).Returns(true);
            _ldapAuthenticationProviderMock.Setup(m => m.Authenticate(It.Is<string>(s => s == "admin3"), It.IsAny<string>())).Returns(true);
            _ldapAuthenticationProviderMock.Setup(m => m.Authenticate(It.Is<string>(s => s == "user"), It.IsAny<string>())).Returns(false);
            _memberServiceMock = new Mock<IMemberService>();
            // Registered user
            _memberServiceMock.Setup(m => m.GetUserByUsernameAsync("admin")).ReturnsAsync(new User()
            {
                Id = 1,
                UserName = "admin",
                Name = "Admin",
                EMail = "admin@gravity.com",
                Role = RoleType.Admin
            });
            // Registered but empty user
            _memberServiceMock.Setup(m => m.GetUserByUsernameAsync("admin2")).ReturnsAsync(new User()
            {
                Id = 1,
                UserName = "admin2",
            });
            // Not Registered user
            _memberServiceMock.Setup(m => m.RegisterNewUserAsync("admin3")).ReturnsAsync(1);
            _memberServiceMock.SetupSequence(m => m.GetUserByUsernameAsync("admin3"))
                .ReturnsAsync((User)null)
                .ReturnsAsync(new User()
                {
                    Id = 1,
                    UserName = "admin3",
                    Name = "Admin 3",
                    EMail = "admin3@gravity.com"
                });
            
            _userStateWrapperNullMock = new Mock<IUserStateWrapper>();
            _userStateWrapperMock = new Mock<IUserStateWrapper>();
            _userStateWrapperMock.Setup(m => m.GetUserState()).Returns(new UserState()
            {
                Id = 1,
                UserName = "admin",
                Name = "Admin",
                EMail = "admin@gravity.com",
                Role = RoleType.Admin
            });

            _memberServiceMock.Setup(m => m.GetUserByIdAsync(1)).ReturnsAsync(new User()
            {
                Id = 1,
                UserName = "admin",
                Name = "Admin",
                EMail = "admin@gravity.com",
                Role = RoleType.Admin
            });
            _userStateWrapperNotRegisteredMock = new Mock<IUserStateWrapper>();
            _userStateWrapperNotRegisteredMock.Setup(m => m.GetUserState()).Returns(new UserState()
            {
                Id = 1,
                UserName = "admin",
            });
            _controllerWitNullUserState = new MemberController(_testLogger, _signinManagerMock.Object, _ldapAuthenticationProviderMock.Object, _memberServiceMock.Object, _userStateWrapperNullMock.Object);
            _controllerWithUserState = new MemberController(_testLogger, _signinManagerMock.Object, _ldapAuthenticationProviderMock.Object, _memberServiceMock.Object, _userStateWrapperMock.Object);
            _controllerWithNotRegisteredUserState = new MemberController(_testLogger, _signinManagerMock.Object, _ldapAuthenticationProviderMock.Object, _memberServiceMock.Object, _userStateWrapperNotRegisteredMock.Object);
        }

        [TearDown]
        public void TestTearDown()
        {
            _testLogger = null;
            _signinManagerMock = null;
            _ldapAuthenticationProviderMock = null;
            _memberServiceMock = null;
            _userStateWrapperNullMock = null;
            _userStateWrapperMock = null;
            _userStateWrapperNotRegisteredMock = null;
            _controllerWitNullUserState.Dispose();
            _controllerWithUserState.Dispose();
            _controllerWithNotRegisteredUserState.Dispose();
        }
        
        [Test]
        public void Model_Logon_InvalidModel_Returns_ModelStateError()
        {
            var model = new LogonViewModel
            {
                UserName = "",
                Password = ""
            };
            var context = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
 
            var valid = Validator.TryValidateObject(model, context, validationResults, true);
 
            Assert.False(valid);
            Assert.AreEqual(2, validationResults.FindAll(i => i.ErrorMessage.Length > 0).Count);
        }
        
        [Test]
        public void Controller_Logon_WithNullUserState_Returns_ViewState()
        {
            var result = _controllerWitNullUserState.Logon();
            
            Assert.NotNull(result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }
        
        [Test]
        public void Controller_Logon_WithUserState_Returns_RedirectToActionMemberIndex()
        {
            var result = _controllerWithUserState.Logon();
            
            Assert.NotNull(result);
            
            Assert.AreEqual(typeof(RedirectToActionResult), result.GetType());
            var redirectToActionResult = (RedirectToActionResult) result;
            
            Assert.AreEqual("Home", redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }
        
        [Test]
        public void Controller_Logon_WithNotRegisteredUserState_Returns_RedirectToActionMemberIndex()
        {
            var result = _controllerWithNotRegisteredUserState.Logon();
            
            Assert.NotNull(result);
            
            Assert.AreEqual(typeof(RedirectToActionResult), result.GetType());
            var redirectToActionResult = (RedirectToActionResult) result;
            
            Assert.AreEqual("Member", redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }
        
        [Test]
        public async Task Controller_Logon_InvalidUser_Returns_ModelStateError()
        {
            var model = new LogonViewModel
            {
                UserName = "user",
                Password = "password"
            };
            var result = await _controllerWitNullUserState.Logon(model);
            
            Assert.NotNull(result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            var viewResult = (ViewResult) result;
            Assert.NotNull(viewResult.ViewData.ModelState[Constants.ModelStateCustomErrorKey].Errors);
            Assert.AreEqual(1, viewResult.ViewData.ModelState[Constants.ModelStateCustomErrorKey].Errors.Count);
            Assert.True(viewResult.ViewData.ModelState[Constants.ModelStateCustomErrorKey].Errors.Any(p => p.ErrorMessage.Equals(MemberController.InvalidLogonMessage)));
        }
        
        [Test]
        public async Task Controller_Logon_ValidAndRegisteredUser_Returns_RedirectToActionHomeIndex()
        {
            var model = new LogonViewModel
            {
                UserName = "admin",
                Password = "password"
            };
            var result = await _controllerWitNullUserState.Logon(model);
            Assert.NotNull(result);
            
            Assert.AreEqual(typeof(RedirectToActionResult), result.GetType());
            var redirectToActionResult = (RedirectToActionResult) result;
            
            Assert.AreEqual("Home", redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }
        
        [Test]
        public async Task Controller_Logon_ValidAndNotProperlyRegisteredUser_Returns_RedirectToActionMemberIndex()
        {
            var model = new LogonViewModel
            {
                UserName = "admin2",
                Password = "password"
            };
            var result = await _controllerWitNullUserState.Logon(model);
            Assert.NotNull(result);
            
            Assert.AreEqual(typeof(RedirectToActionResult), result.GetType());
            var redirectToActionResult = (RedirectToActionResult) result;
            
            Assert.AreEqual("Member", redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }
        
        [Test]
        public async Task Controller_Logon_ValidAndNotRegisteredUser_Returns_RedirectToActionMemberIndex()
        {
            var model = new LogonViewModel
            {
                UserName = "admin3",
                Password = "password"
            };
            var result = await _controllerWitNullUserState.Logon(model);
            Assert.NotNull(result);
            
            Assert.AreEqual(typeof(RedirectToActionResult), result.GetType());
            var redirectToActionResult = (RedirectToActionResult) result;
            
            Assert.AreEqual("Member", redirectToActionResult.ControllerName);
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }
        
        [Test]
        public async Task Controller_Index_WithoutUser_RedirectToActionMemberLogon()
        {
            var result = await _controllerWitNullUserState.Index();
            Assert.NotNull(result);
            
            Assert.AreEqual(typeof(RedirectToActionResult), result.GetType());
            var redirectToActionResult = (RedirectToActionResult) result;
            
            Assert.AreEqual("Member", redirectToActionResult.ControllerName);
            Assert.AreEqual("Logon", redirectToActionResult.ActionName);
        }
        
        [Test]
        public async Task Controller_Index_WitUser_ReturnsModel()
        {
            var result = await _controllerWithUserState.Index();
            Assert.NotNull(result);
            
            Assert.NotNull(result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            var viewResult = (ViewResult) result;
            
            Assert.NotNull(viewResult.Model);
            Assert.AreEqual(typeof(UserProfileViewModel), viewResult.Model.GetType());

            var model = (UserProfileViewModel) viewResult.Model;
            
            Assert.AreEqual("admin", model.UserName);
            Assert.AreEqual("Admin", model.Name);
        }
        
        [Test]
        public async Task Controller_Index_WitUserAndDifferentIdInModel_Returns_ModelStateError()
        {
            var model = new UserProfileViewModel()
            {
                Id = 2,
            };
            var result = await _controllerWithUserState.Index(model);
            Assert.NotNull(result);
            
            Assert.NotNull(result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            var viewResult = (ViewResult) result;
            
            Assert.NotNull(viewResult.Model);
            Assert.AreEqual(typeof(UserProfileViewModel), viewResult.Model.GetType());
            
            Assert.NotNull(viewResult.ViewData.ModelState[Constants.ModelStateCustomErrorKey].Errors);
            Assert.AreEqual(1, viewResult.ViewData.ModelState[Constants.ModelStateCustomErrorKey].Errors.Count);
            Assert.True(viewResult.ViewData.ModelState[Constants.ModelStateCustomErrorKey].Errors.Any(p => p.ErrorMessage.Equals(MemberController.NotMachingUserId)));
        }
    }
}