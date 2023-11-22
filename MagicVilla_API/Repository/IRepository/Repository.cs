using MagicVilla_API.Data;
using MagicVilla_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository.IRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {


        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }






        public async Task CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();
        }

        // Expression<Func<Villa, bool>> filter = null ---> Bu parametre, Villa türündeki nesneleri filtrelemek için bir lambda ifadesini içeren bir Expression türünden bir filtre ifadesini temsil eder. Bu parametre, varsayılan olarak null olarak atanmıştır, yani filtre kullanılmadan çağrılabileceği anlamına gelir. Bu filtre, Villa türündeki nesnelerin belirli bir koşula göre seçilmesini sağlar.

        // bool tracked = true ---> Bu parametre, takip edilen (tracked) nesnelerin kullanılıp kullanılmayacağını belirler. Entity Framework gibi ORM (Object-Relational Mapping) araçları, nesnelerin veritabanı değişikliklerini takip etmelerine olanak tanır. Bu parametre, takip edilen nesnelerin kullanılmasını kontrol eder. Varsayılan olarak true olarak atanmıştır, yani takip edilen nesneler kullanılır.

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }



        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }



        public async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await SaveAsync();
        }



        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }


    }

}

