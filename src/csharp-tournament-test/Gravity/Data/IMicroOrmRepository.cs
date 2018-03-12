using System.Collections.Generic;

namespace Gravity.Data
{
    public interface IMicroOrmRepository<TEntity, in TId> where TEntity: IEntity<TId>
        where TId : struct
    {

        void Delete(TId id, bool saveAfter = false, bool async = false, int? commandTimeout = null);

        void Delete(TEntity entity, bool saveAfter = false, bool async = false, int? commandTimeout = null);

        void Insert(TEntity entity, bool saveAfter = false, bool async = false, int? commandTimeout = null);

        void Update(TEntity entity, bool saveAfter = false, bool async = false, int? commandTimeout = null);

        void BulkInsert(IEnumerable<TEntity> entities, bool saveAfter = false, bool async = false, int? commandTimeout = null);

        void BulkDelete(IEnumerable<object> ids, bool saveAfter = false, bool async = false, int? commandTimeout = null);

        void BulkDelete(IEnumerable<TEntity> entities, bool saveAfter = false, bool async = false, int? commandTimeout = null);

        void BulkUpdate(IEnumerable<TEntity> entities, bool saveAfter = false, bool async = false, int? commandTimeout = null);

        /// <summary>
        ///     Get <see cref="TEntity">TEntity</see> by Tid.
        /// </summary>
        TEntity Get(TId id, int? commandTimeout = null);

        /// <summary>
        ///     Get all <see cref="TEntity">TEntity</see> for underlying table.
        /// </summary>
        IEnumerable<TEntity> GetAll(int? commandTimeout = null);

        /// <summary>
        ///     Save to commit changes to persistent store...
        /// </summary>
        void Commit(bool async = false, int? commandTimeout = null);
    }
}