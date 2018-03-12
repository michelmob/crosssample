using System;
using System.Data;
using System.Threading.Tasks;

namespace Gravity.Data
{
    public interface IUnitOfWork : IDisposable
    {
        int Commit();
        
        Task<int> CommitAsync();

        IDbTransaction BeginDbTransaction();
    }
}