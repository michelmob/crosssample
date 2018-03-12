using System;
using System.Collections.Generic;
using System.Linq;
using Gravity.Manager.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using SlowMember;

namespace Gravity.Manager.Web.Application
{
    public static class Extensions
    {
        public static ObjectResult ToValidationErrorResult(this ModelStateDictionary modelState)
        {
            modelState = modelState ?? throw new ArgumentNullException(nameof(modelState));

            var result = new ValidationErrorsViewModel
            {
                Message = "Invalid request data",
                Errors = modelState.Keys.SelectMany(key => modelState[key].Errors.Select(x =>
                    new ValidationErrorViewModel
                    {
                        Property = key,
                        Message = x.ErrorMessage
                    })).ToList()
            };

            return new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        public static List<SelectListItem> ToSelectList<TItem>(this List<TItem> genericList, string keyFieldName = "Id", string textFieldName = "Name")
        {
            if (genericList == null) throw new ArgumentNullException(nameof(genericList));
            if (genericList.Count == 0) return new List<SelectListItem>();
            var type = typeof(TItem);
            var objectDescription = type.GetObjectDescription();
            var keyField = objectDescription.MemberDescriptions.FirstOrDefault(d => d.Name == keyFieldName);
            if (keyField == null) throw new ArgumentOutOfRangeException(nameof(keyField));
            var textField = objectDescription.MemberDescriptions.FirstOrDefault(d => d.Name == textFieldName);
            if (textField == null) throw new ArgumentOutOfRangeException(nameof(textField));
            var selectList = new List<SelectListItem>(genericList.Count);
            foreach (var item in genericList)
            {
                var si = new SelectListItem();
                var value = keyField.GetValue(item);
                si.Value = value == null ? "0" : value.ToString();
                var text = textField.GetValue(item);
                si.Text = text == null ? "0" : text.ToString();
                selectList.Add(si);
            }

            return selectList;
        }
    }
}