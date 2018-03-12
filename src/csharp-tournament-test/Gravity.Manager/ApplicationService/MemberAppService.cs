using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gravity.Diagnostics;
using Gravity.Manager.Domain;
using Gravity.Manager.Domain.Organizations;

namespace Gravity.Manager.ApplicationService
{
    public class MemberAppService : IMemberAppService
    {
        private readonly ILogger _logger;
        private readonly IMemberUnitOfWork _memberUnitOfWork;

        public MemberAppService(ILogger logger
            , IMemberUnitOfWork memberUnitOfWork
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(memberUnitOfWork));
            _memberUnitOfWork = memberUnitOfWork ?? throw new ArgumentNullException(nameof(memberUnitOfWork));
        }

        public Task<User> GetUserByUsernameAsync(string userName)
        {
            return _memberUnitOfWork.UserRepository.GetUserByUsernameAsync(userName);
        }

        public Task<bool> CheckUserByUsernameAsync(string userName)
        {
            return _memberUnitOfWork.UserRepository.CheckUserByUsernameAsync(userName);
        }

        public async Task<int> RegisterNewUserAsync(string userName)
        {
            return await _memberUnitOfWork.UserRepository.RegisterNewUserAsync(userName);
        }

        public Task<User> GetUserByIdAsync(long id)
        {
            return _memberUnitOfWork.UserRepository.GetAsync(id);
        }

        public Task<int> UpdateUserAsync(User user)
        {
            return _memberUnitOfWork.UserRepository.UpdateAsync(user);
        }

        public async Task<MemberList> GetMemberListAsync()
        {
            var users = await _memberUnitOfWork.UserRepository.GetAllAsync();
            var organizations = await _memberUnitOfWork.OrganizationRepository.GetAllAsync();
            var memberList = new MemberList()
            {
                Users = users,
                Organizations = organizations
            };
            return memberList;
        }

        public async Task<int> DeleteMemberAsync(long id)
        {
            return await _memberUnitOfWork.UserRepository.DeleteAsync(id);
        }

        public async Task<List<Organization>> GetAllOrganizationsAsync()
        {
            return await _memberUnitOfWork.OrganizationRepository.GetAllAsync();
        }

        public async Task<Organization> GetOrganizationByIdAsync(long id)
        {
            return await _memberUnitOfWork.OrganizationRepository.GetAsync(id);
        }

        public async Task<int> UpdateOrganizationAsync(Organization organization)
        {
            return await _memberUnitOfWork.OrganizationRepository.UpdateAsync(organization);
        }

        public async Task<int> DeleteOrganizationAsync(long id)
        {
            return await _memberUnitOfWork.OrganizationRepository.DeleteAsync(id);
        }

        public void Dispose()
        {
            _memberUnitOfWork?.Dispose();
        }
    }
}