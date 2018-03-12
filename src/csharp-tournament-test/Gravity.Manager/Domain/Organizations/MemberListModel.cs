using System.Collections.Generic;

namespace Gravity.Manager.Domain.Organizations
{
    public class MemberList
    {
        public List<User> Users { get; set; }
        
        public List<Organization> Organizations { get; set; }
        
    }
}