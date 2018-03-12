using System.Collections.Generic;
using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Data.Models
{
    public class MemberList
    {
        public List<User> Users { get; set; }
        
        public List<Organization> Organizations { get; set; }
        
    }
}