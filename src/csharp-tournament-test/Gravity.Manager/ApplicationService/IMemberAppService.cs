using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gravity.Manager.Domain.Organizations;

namespace Gravity.Manager.ApplicationService
{
    public interface IMemberAppService: IDisposable
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
