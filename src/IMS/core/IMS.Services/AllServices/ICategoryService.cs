using IMS.Models.Entities;
using System.Threading.Tasks;

namespace IMS.AllServices
{
    public interface ICategoryService
    {
        Task AddAsync(Category category, string aspUser);
    }
}