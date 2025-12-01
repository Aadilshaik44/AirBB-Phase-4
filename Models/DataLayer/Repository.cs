using Microsoft.EntityFrameworkCore;

namespace AirBB.Models.DataLayer
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AirBnbContext _context;
        protected DbSet<T> _dbSet;

        public Repository(AirBnbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual IEnumerable<T> List(QueryOptions<T> options)
        {
            IQueryable<T> query = _dbSet;

            foreach (string include in options.Includes)
            {
                query = query.Include(include);
            }

            if (options.HasWhere)
            {
                query = query.Where(options.Where);
            }

            if (options.HasOrderBy)
            {
                if (options.OrderByDirection == "asc")
                    query = query.OrderBy(options.OrderBy);
                else
                    query = query.OrderByDescending(options.OrderBy);
            }

            return query.ToList();
        }

        public virtual T? Get(int id) => _dbSet.Find(id);

        public virtual T? Get(QueryOptions<T> options)
        {
            IQueryable<T> query = _dbSet;

            foreach (string include in options.Includes)
            {
                query = query.Include(include);
            }

            if (options.HasWhere)
                query = query.Where(options.Where);

            return query.FirstOrDefault();
        }

        public virtual void Insert(T entity) => _dbSet.Add(entity);
        public virtual void Update(T entity) => _dbSet.Update(entity);
        public virtual void Delete(T entity) => _dbSet.Remove(entity);
        public virtual void Save() => _context.SaveChanges();
        public virtual int Count() => _dbSet.Count();
    }
}