using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gravity.Manager.Domain.Organizations;

namespace Gravity.Manager.Web.Models
{
    public class UserEditViewModel
    {
        public UserEditViewModel()
        {
            
        }

        public UserEditViewModel(User user)
        {
            FromUser(user);
        }
        
        public long Id { get; set; }
        
        public long? OrganizationId { get; set; }
        
        [MinLength(4)]
        [StringLength(100)]
        public string UserName { get; set; }
        
        [MinLength(4)]
        [StringLength(100)]
        public string Name { get; set; }
        
        [MinLength(4)]
        [StringLength(100)]
        public string EMail { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Role is Required")]
        public RoleType Role { get; set; }

        public List<Organization> Organizations { get; set; }

        public void FromUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            Id = user.Id;
            OrganizationId = user.OrganizationId;
            UserName = user.UserName;
            Name = user.Name;
            EMail = user.EMail;
            Role = user.Role;
        }
        
        public User ChangeUser(User cleanUser)
        {
            if (cleanUser == null)
            {
                throw new ArgumentNullException(nameof(cleanUser));
            }
            
            cleanUser.Name = Name;
            cleanUser.EMail = EMail;
            cleanUser.OrganizationId = OrganizationId;
            cleanUser.Role = Role;
            return cleanUser;
        }
    }
}