using Gravity.Manager.Web.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gravity.Manager.Web.Controllers
{
    [Authorize]
    public class BaseMvcController : Controller
    {
        public void AppendViewErrorMessage(string errorMessage)
        {
            ViewData[Constants.ViewDataErrorMessageKey] = $"{ViewData[Constants.ViewDataErrorMessageKey]}\n{errorMessage}";
        }
        
        public string GetTempDataErrorMessage()
        {
            return TempData[Constants.ViewDataErrorMessageKey] as string;
        }

        public void SetTempDataErrorMessage(string errorMessage)
        {
            TempData[Constants.ViewDataErrorMessageKey]= errorMessage;
        }
        
        public void CheckAndSetErrorMessage()
        {
            var errorMessage = GetTempDataErrorMessage();
            if (!string.IsNullOrWhiteSpace(errorMessage)) AppendViewErrorMessage(errorMessage);
        }
    }
}