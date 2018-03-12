using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Gravity.Data.EF
{
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId> 
        where TEntity : class, IEntity<TId> 
        where TId : struct 
    {
        protected GenericRepository(DbContext context
            // , bool disableChangeTracking = false,
            // bool disableLazyLoading = false,
            // bool disableProxyCreation = false, bool useDatabaseNullSemantics = false, bool disableValidateOnSave = false
            )
        {
            // ReSharper disable once JoinNullCheckWithUsage : ReSharper issue
            if (context == null) throw new ArgumentNullException("Database Context can not be null!");
            
            Context = context;
            Set = context.Set<TEntity>();

            // Context.Configuration.AutoDetectChangesEnabled = !disableChangeTracking;
            // Context.Configuration.LazyLoadingEnabled = !disableLazyLoading;
            // Context.Configuration.ProxyCreationEnabled = !disableProxyCreation;
            // Context.Configuration.UseDatabaseNullSemantics = !useDatabaseNullSemantics;
            // Context.Configuration.ValidateOnSaveEnabled = !disableValidateOnSave;
        }

        private DbContext Context { get; }

        private DbSet<TEntity> Set { get; }

        #region Public Methods
        
        protected IQueryable<TEntity> AsQueryable()
        {
            return Set;
        }
        
        public void Insert(TEntity entity, bool commitAfter = false)
        {
            Set.Add(entity);
            if (commitAfter) Commit();
        }

        public Task<int> InsertAsync(TEntity entity)
        {
            Insert(entity);
            return CommitAsync();
        }

        public void Update(TEntity entity, bool commitAfter = false)
        {
            Set.Attach(entity);
            if (commitAfter) Commit();
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            Update(entity);
            return CommitAsync();
        }

        public void Delete(TId id, bool commitAfter = false) 
        {
            var entityToDelete = Set.Find(id);
            Delete(entityToDelete);
            if (commitAfter) Commit();
        }

        public Task<int> DeleteAsync(TId id)
        {
            Delete(id);
            return CommitAsync();
        }

        public void Delete(TEntity entity, bool commitAfter = false)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                Set.Attach(entity);
            }
            Set.Remove(entity);
            if (commitAfter) Commit();
        }

        public Task<int> DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return CommitAsync();
        }

        public void BulkInsert(IEnumerable<TEntity> entities, bool commitAfter = false)
        {
            Set.AddRange(entities);
            if (commitAfter) Commit();
        }

        public Task<int> BulkInsertAsync(IEnumerable<TEntity> entities)
        {
            BulkInsert(entities);
            return CommitAsync();
        }
        
        public void BulkUpdate(IEnumerable<TEntity> entities, bool commitAfter = false)
        {
            foreach (var entity in entities)
            {
                var existing = GetLocalExistingEntity(entity);
                if (existing != null)
                {
                    Context.Entry(existing).State = EntityState.Detached;
                }
                Set.Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
            }
            if (commitAfter) Commit();
        }

        public Task<int> BulkUpdateAsync(IEnumerable<TEntity> entities)
        {
            BulkUpdate(entities);
            return CommitAsync();
        }

        public void BulkDelete(IEnumerable<object> ids, bool commitAfter = false)
        {
            foreach (var id in ids)
            {
                var entityToDelete = Set.Find(id);
                if (entityToDelete != null)
                    Delete(entityToDelete);
            }
            if (commitAfter) Commit();
        }

        public Task<int> BulkDeleteAsync(IEnumerable<object> ids)
        {
            BulkDelete(ids);
            return CommitAsync();
        }

        public void BulkDelete(IEnumerable<TEntity> entities, bool commitAfter = false)
        {
            foreach (var entity in entities)
            {
                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    Set.Attach(entity);
                }
                Set.Remove(entity);
            }
            if (commitAfter) Commit();
        }

        public Task<int> BulkDeleteAsync(IEnumerable<TEntity> entities)
        {
            BulkDelete(entities);
            return CommitAsync();
        }

        public TEntity GetLocalExistingEntity(TEntity entity)
        {
            if (!Set.Local.Any()) return null;
            var primaryKey = entity.Id;
            var existing = Set.Local.FirstOrDefault(f => f.Id.Equals(primaryKey));
            return existing;
        }

        public TEntity Get(TId id)
        {
            return Set.Find(id);
        }
        
        public Task<TEntity> GetAsync(TId id)
        {
            return Set.FindAsync(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            var query = AsQueryable();
            var list = query.ToList();
            return list;
        }
        
        public Task<List<TEntity>> GetAllAsync()
        {
            var query = AsQueryable();
            return query.ToListAsync();            
        }

        public TEntity Find(Expression<Func<TEntity, bool>> match)
        {
            return Set.Where(match).FirstOrDefault();
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
        {
            return Set.Where(match).FirstOrDefaultAsync();
        }

        public List<TEntity> FindAll(Expression<Func<TEntity, bool>> match)
        {
            return Set.Where(match).ToList();
        }

        public Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match)
        {
            return Set.Where(match).ToListAsync();
        }
        
        public async Task<DataPage<TEntity>> GetPageAsync<TInclude, TOrderBy>(int pageIndex, int pageSize,
            Expression<Func<TEntity, TInclude>> include = null,
            Expression<Func<TEntity, TOrderBy>> orderBy = null,
            bool descending = false)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("Page index can not be negative.", nameof(pageIndex));
            }
            
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException("Page size must be positive.", nameof(pageSize));
            }
            
            var count = await Set.CountAsync();

            IQueryable<TEntity> set = Set;

            if (include != null)
            {
                set = set.Include(include);
            }

            if (orderBy != null)
            {
                set = descending ? set.OrderByDescending(orderBy) : set.OrderBy(orderBy);
            }

            var items = await set
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new DataPage<TEntity>(items, count, pageSize, pageIndex);
        }

        public void Commit()
        {
            Context.SaveChanges();
        }
        
        public async Task<int> CommitAsync()
        {
            return await Context.SaveChangesAsync();
        }

        #endregion

        #region Private Methods

        #endregion

        #region IDisposable

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
