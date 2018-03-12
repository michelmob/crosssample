using System;
using Gravity.Data;

namespace Gravity.Manager.ApplicationService
{
    public abstract class BaseAppService 
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected BaseAppService(IUnitOfWork  unitOfWork)
        {
            this.UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

    }
}
