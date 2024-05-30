using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace votingapp.Infrastructure
{
    /// <summary>
    /// Generic repository for performing CRUD operations on entities.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext DbContext;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="configuration">The configuration.</param>
        public Repository(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #region Public Methods

        /// <summary>
        /// Gets all entities asynchronously.
        /// </summary>
        /// <returns>A list of entities.</returns>
        public async Task<List<T>> GetAllAsyncList()
        {
            return await DbContext.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public T Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                DetachAllEntries();
                DbContext.Set<T>().Add(entity);
                DbContext.SaveChanges();
                return entity;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism to be implemented)
                throw new DataException("Error adding entity to the database.", ex);
            }
        }

        /// <summary>
        /// Adds a new entity to the database asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                DbContext.Set<T>().Add(entity);
                await DbContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism to be implemented)
                throw new DataException("Error adding entity to the database asynchronously.", ex);
            }
        }

        /// <summary>
        /// Finds an entity that matches the specified criteria.
        /// </summary>
        /// <param name="match">The expression to match.</param>
        /// <returns>The found entity.</returns>
        public T Find(Expression<Func<T, bool>> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            try
            {
                return DbContext.Set<T>().FirstOrDefault(match);
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism to be implemented)
                throw new DataException("Error finding entity in the database.", ex);
            }
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="updated">The updated entity.</param>
        /// <param name="key">The key of the entity to update.</param>
        /// <returns>The updated entity.</returns>
        public T Update(T updated, int key)
        {
            if (updated == null)
                throw new ArgumentNullException(nameof(updated));

            try
            {
                var existing = DbContext.Set<T>().Find(key);
                if (existing != null)
                {
                    DbContext.Entry(existing).CurrentValues.SetValues(updated);
                    DbContext.SaveChanges();
                }
                return existing;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism to be implemented)
                throw new DataException("Error updating entity in the database.", ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Detaches all tracked entries in the DbContext.
        /// </summary>
        private void DetachAllEntries()
        {
            foreach (var entity in DbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
            {
                DbContext.Entry(entity.Entity).State = EntityState.Detached;
            }
        }

        #endregion
    }
}
