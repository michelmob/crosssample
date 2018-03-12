using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Gravity.Data.Dapper
{
    public class DapperMicroOrmRepository<TEntity, TId> : IMicroOrmRepository<TEntity, TId> where TEntity : class, IEntity<TId> where TId : struct
    {

        public DapperMicroOrmRepository(IDbConnection connection, string entityName = "")
        {
            _connection = connection;
            SetEntityName(entityName);
        }

        public DapperMicroOrmRepository(IDbTransaction transaction, string entityName = "")
        {
            _transaction = transaction;
            _connection = _transaction.Connection;
            SetEntityName(entityName);
        }

        private string _entityName;
        private void SetEntityName(string entityName = "") {
            if (entityName.Trim().Length > 0 ) {
                _entityName = entityName;
            } else {
                _entityName = nameof(TEntity);
            }
        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public void BulkDelete(IEnumerable<object> ids, bool saveAfter = false, bool async = false, int? commandTimeout = null)
        {
            var sb = new StringBuilder(string.Format("delete from {0} where Id in(", _entityName));
            var aids = ids.AsList().ToArray();
            for (int i = 0; i < aids.Length -1; i++)
            {
                if (i != aids.Length -1) {
                    sb.AppendLine(string.Format("{0}, ", aids[i]));
                } else {
                    sb.AppendLine(string.Format("{0})", aids[i]));
                }
                
            }
            var delete = sb.ToString();
            _connection.Execute(delete, null, _transaction, commandTimeout);
            SaveAfter(saveAfter);
        }

        public void BulkDelete(IEnumerable<TEntity> entities, bool saveAfter = false, bool async = false, int? commandTimeout = null)
        {
            // var sb = new StringBuilder(string.Format("delete from {0} where Id in(", _entityName));
            // var elist = entities.AsList();
            // TId[] aids = new TId[elist.Count];
            // for (int i = 0; i < elist.Count -1; i++)
            // {
            //     aids[i] = elist[i].Id;
            // }
            // for (int i = 0; i < aids.Length -1; i++)
            // {
            //     if (i != aids.Length -1) {
            //         sb.AppendLine(string.Format("{0}, ", aids[i]));
            //     } else {
            //         sb.AppendLine(string.Format("{0})", aids[i]));
            //     }
            // }
            // var delete = sb.ToString();
            // _connection.Execute(delete, null, _transaction, commandTimeout);
            _connection.Delete(entities, _transaction, commandTimeout);
            SaveAfter(saveAfter);
        }

        public void BulkInsert(IEnumerable<TEntity> entities, bool saveAfter = false, bool async = false, int? commandTimeout = null)
        {
            _connection.Insert(entities, _transaction, commandTimeout);
            SaveAfter(saveAfter);
        }

        public void BulkUpdate(IEnumerable<TEntity> entities, bool saveAfter = false, bool async = false, int? commandTimeout = null)
        {
            _connection.Update(entities, _transaction, commandTimeout);
            SaveAfter(saveAfter);
        }

        public void Delete(TId id, bool saveAfter = false, bool async = false, int? commandTimeout = null)
        {
            var delete = string.Format("delete from {0} where Id={1}", _entityName, id);
            _connection.Execute(delete, null, _transaction, commandTimeout);
            SaveAfter(saveAfter);
        }

        public void Delete(TEntity entity, bool saveAfter = false, bool async = false, int? commandTimeout = null)
        {
            _connection.Delete(entity, _transaction, commandTimeout);
            SaveAfter(saveAfter);
        }

        public TEntity Get(TId id, int? commandTimeout = null)
        {
            var result = _connection.Get<TEntity>(id, _transaction, commandTimeout);
            return result;
        }

        public IEnumerable<TEntity> GetAll(int? commandTimeout = null)
        {
            var result = _connection.GetAll<TEntity>(_transaction, commandTimeout);
            return result;
        }

        public void Insert(TEntity entity, bool saveAfter = false, bool async = false, int? commandTimeout = null)
        {
            _connection.Insert(entity, _transaction, commandTimeout);
            SaveAfter(saveAfter);
        }

        public void Commit(bool async = false, int? commandTimeout = null)
        {
            _transaction.Commit();
        }

        public void Update(TEntity entity, bool saveAfter = false, bool async = false, int? commandTimeout = null)
        {
            _connection.Update(entity, _transaction, commandTimeout);
            SaveAfter(saveAfter);
        }

        private void SaveAfter(bool saveAfter) {
            if (saveAfter) {
                Commit();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (_connection != null) {
                        _connection.Dispose();
                        _connection = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DapperGenericRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion


    }

}