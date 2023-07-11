using IMS.DataAccess.Repositories;
using IMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public void Add()
        {
            var cat = new Category { Name = "Category-1", Description = "*", BusinessId = "Nothing" };
            _categoryRepository.Add(cat);
        }

        public async Task<Category> GetByIdAsync(long id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }
    }
}
