using System;
using System.Data;
using System.Threading.Tasks;

namespace Gravity.Data
{
    public abstract class UnitOfWorkBase<TContext> : IUnitOfWork where TContext : class, IDisposable
    {
        protected UnitOfWorkBase(TContext context)
        {
            Context = context;
        }

        protected TContext Context { get; }

        /// <summary>Performs commit operation on pending transaction for all repositories containing in current UnitOfWork implementation.</summary>
        public abstract int Commit();
        
        /// <summary>Performs commit operation on pending transaction for all repositories containing in current UnitOfWork implementation.</summary>
        public abstract Task<int> CommitAsync();

        public abstract IDbTransaction BeginDbTransaction();

        #region IDisposable

        private bool _disposed;

        /// <summary>
        ///     Custom dispose method for internal use
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context?.Dispose();
                }
            }
            _disposed = true;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}