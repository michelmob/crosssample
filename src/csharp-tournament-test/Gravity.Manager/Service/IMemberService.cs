using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Data.Models;

namespace Gravity.Manager.Service
{
    public interface IMemberService: IDisposable
    {
        Task<User> GetUserByUsernameAsync(string userName);
        
        Task<bool> CheckUserByUsernameAsync(string userName);

        Task<int> RegisterNewUserAsync(string userName);
        
        Task<User> GetUserByIdAsync(long id);
        
        Task<int> UpdateUserAsync(User user);
        
        Task<MemberList> GetMemberListAsync();
        
        Task<int> DeleteMemberAsync(long id);
        
        Task<List<Organization>> GetAllOrganizationsAsync();
        
        Task<Organization> GetOrganizationByIdAsync(long id);
        
        Task<int> UpdateOrganizationAsync(Organization organization);
        
        Task<int> DeleteOrganizationAsync(long id);
    }
}
