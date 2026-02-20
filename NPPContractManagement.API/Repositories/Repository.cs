using Microsoft.EntityFrameworkCore;
using NPPContractManagement.API.Data;
using System.Linq.Expressions;

namespace NPPContractManagement.API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<PaginatedResult<T>> GetPaginatedAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? sortBy = null,
            string sortDirection = "asc",
            string? searchTerm = null)
        {
            var query = _dbSet.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = ApplySearchFilter(query, searchTerm);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = ApplySorting(query, sortBy, sortDirection);
            }

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        protected virtual IQueryable<T> ApplySearchFilter(IQueryable<T> query, string searchTerm)
        {
            var entityType = typeof(T);
            var parameter = Expression.Parameter(entityType, "x");
            Expression? searchExpression = null;

            // Get searchable properties (string properties)
            var searchableProperties = entityType.GetProperties()
                .Where(p => p.PropertyType == typeof(string) &&
                           (p.Name.Contains("Name") || p.Name.Contains("Description") ||
                            p.Name.Contains("Email") || p.Name.Contains("Title")))
                .ToList();

            foreach (var property in searchableProperties)
            {
                var propertyAccess = Expression.Property(parameter, property);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var searchValue = Expression.Constant(searchTerm, typeof(string));
                var containsCall = Expression.Call(propertyAccess, containsMethod!, searchValue);

                searchExpression = searchExpression == null
                    ? containsCall
                    : Expression.OrElse(searchExpression, containsCall);
            }

            if (searchExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        protected virtual IQueryable<T> ApplySorting(IQueryable<T> query, string sortBy, string sortDirection)
        {
            var entityType = typeof(T);
            var parameter = Expression.Parameter(entityType, "x");

            var property = entityType.GetProperty(sortBy);
            if (property == null) return query;

            var propertyAccess = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);

            var methodName = sortDirection.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { entityType, property.PropertyType },
                query.Expression,
                Expression.Quote(lambda));

            return query.Provider.CreateQuery<T>(resultExpression);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
}
