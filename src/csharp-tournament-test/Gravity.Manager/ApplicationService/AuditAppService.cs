using System;
using System.Threading.Tasks;
using Gravity.Data;
using Gravity.Manager.Domain.Audits;

// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleAnonymousFunction
namespace Gravity.Manager.ApplicationService
{
    public class AuditAppService : BaseAppService, IAuditAppService
    {
        private readonly IAuditRepository auditRepository;

        public AuditAppService(IUnitOfWork unitOfWork,IAuditRepository auditRepository) : base(unitOfWork)
        {
            this.auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
        }

        //private readonly IAuditUnitOfWork _context;

        //public AuditAppService(IAuditUnitOfWork context)
        //{
        //    _context = context ?? throw new ArgumentNullException(nameof(context));
        //}

        public Task<DataPage<AuditEntry>> GetAuditPageAsync(int pageIndex, int pageSize)
        {
            //TODO: Better if we use specifications 

            return auditRepository.GetPageAsync(pageIndex, pageSize,
                include: a => a.User, orderBy: a => a.Date, @descending: true);
        }

        public void Dispose()
        {
            auditRepository?.Dispose();
        }

       
    }
}