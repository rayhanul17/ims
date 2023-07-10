using IMS.Models.Entities;
using NHibernate;

namespace IMS.DataAccess.Repositories
{
    public class CategoryRepository : Repository<Category, long>, ICategoryRepository
    {
        public CategoryRepository(ISession session) : base(session)
        {

        }
    }
}
