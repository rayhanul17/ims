using IMS.Models.Dtos.Categories;
using System.Threading.Tasks;

namespace IMS.AllServices
{
    public interface ICategoryService
    {
        Task AddAsync(CategoryAdd category);
    }
}