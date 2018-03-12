using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Gravity.Data
{
    public interface IGenericRepository<TEntity, in TId>: IDisposable where TEntity : class, IEntity<TId>
        where TId : struct
    {
        /// <summary>
        /// Inserts given entity but not commits changes by default
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="commitAfter">if true calls Commit();</param>
        void Insert(TEntity entity, bool commitAfter = false);
        
        /// <summary>
        /// Inserts given entity then commit changes asynchronously 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> InsertAsync(TEntity entity);
        
        /// <summary>
        /// Updates given entity but not commits changes by default
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="commitAfter"></param>
        void Update(TEntity entity, bool commitAfter = false);
        
        
        /// <summary>
        /// Updates given entity then commit changes asynchronously
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(TEntity entity);
        
        
        /// <summary>
        /// Searchs given id as primarykey and deletes entity but not commits changes by default 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="commitAfter"></param>
        void Delete(TId id, bool commitAfter = false);
        
        
        /// <summary>
        /// Searchs given id as primarykey and deletes entity then commit changes asynchronously
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(TId id);

        /// <summary>
        /// Deletes given entity but not commits changes by default
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="commitAfter"></param>
        void Delete(TEntity entity, bool commitAfter = false);
        
        /// <summary>
        /// Deletes given entity then commit changes asynchronously
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteAsync(TEntity entity);

        /// <summary>
        /// Inserts list of entites but not commits changes by default
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="commitAfter"></param>
        void BulkInsert(IEnumerable<TEntity> entities, bool commitAfter = false);
        
        /// <summary>
        /// Inserts list of entities then commit changes asynchronously
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> BulkInsertAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates list of entities but not commit changes by default
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="commitAfter"></param>
        void BulkUpdate(IEnumerable<TEntity> entities, bool commitAfter = false);
        
        /// <summary>
        /// Updates list of entities then commit changes asynchronously
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> BulkUpdateAsync(IEnumerable<TEntity> entities);
        
        /// <summary>
        /// Searchs given IDs as primarykey and deletes entities but not commits changes by default
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="commitAfter"></param>
        void BulkDelete(IEnumerable<object> ids, bool commitAfter = false);
        
        /// <summary>
        /// Searchs given IDs as primarykey and deletes entities then commit changes asynchronously
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<int> BulkDeleteAsync(IEnumerable<object> ids);
        
        /// <summary>
        /// Deletes list of entities but not commits changes by default
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="commitAfter"></param>
        void BulkDelete(IEnumerable<TEntity> entities, bool commitAfter = false);
        
        /// <summary>
        /// Deletes list of entities then commit changes asynchronously 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> BulkDeleteAsync(IEnumerable<TEntity> entities);
        
        /// <summary>
        /// Gets entity from local context
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity GetLocalExistingEntity(TEntity entity);

        /// <summary>
        /// Gets entity by its Primary Key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Get(TId id);
        
        /// <summary>
        /// Gets entity by its Primary Key asynchronously
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TEntity> GetAsync(TId id);

        /// <summary>
        /// Gets all the entities
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();
        
        /// <summary>
        /// Gets all the entities asynchronously
        /// </summary>
        /// <returns></returns>
        Task<List<TEntity>> GetAllAsync();

        /// <summary>
        /// Finds matching entity by specified expression 
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        TEntity Find(Expression<Func<TEntity, bool>> match);
        
        /// <summary>
        /// Finds matching entity by specified expression asynchronously
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match);

        /// <summary>
        /// Finds matching entities by specified expression 
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        List<TEntity> FindAll(Expression<Func<TEntity, bool>> match);
        
        /// <summary>
        /// Finds matching entities by specified expression asynchronously
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match);

        /// <summary>
        /// Commit changes to the persistent storage
        /// </summary>
        void Commit();
        
        
        /// <summary>
        /// Commit changes to the persistent storage asynchronously
        /// </summary>
        /// <returns></returns>
        Task<int> CommitAsync();

        Task<DataPage<TEntity>> GetPageAsync<TInclude, TOrderBy>(int pageIndex, int pageSize,
            Expression<Func<TEntity, TInclude>> include = null,
            Expression<Func<TEntity, TOrderBy>> orderBy = null,
            bool descending = false);
    }
}