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
            var p1 = new Product { Name = "P1", Description = "None" };
            var p2 = new Product { Name = "P2", Description ="None" };
            var c1 = new Category { Name = "C1" };
            p1.Category = c1;
            p2.Category = c1;
            c1.Products = new List<Product> { p1, p2 };

            _categoryRepository.AddAsync(c1);
        }

        public async Task<Category> GetByIdAsync(long id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }
    }
}
