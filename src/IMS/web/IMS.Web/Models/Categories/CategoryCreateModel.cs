using Autofac;
using IMS.AllServices;
using IMS.Infrastructure.Enums;
using IMS.Models.Dtos.Categories;
using IMS.Models.Entities;
using System.Threading.Tasks;

namespace IMS.Web.Models.Categories
{
    public class CategoryCreateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }
        public Status Status { get; set; }        

        private ILifetimeScope _scope;
        private ICategoryService _categoryService;

        public CategoryCreateModel()
        {

        }

        public CategoryCreateModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public void ResolveDependency(ILifetimeScope scope)
        {
            _scope = scope;
            _categoryService = _scope.Resolve<ICategoryService>();
        }

        internal async Task AddAsync()
        {
            var category = new CategoryAdd { Name = Name, Description = Description, Rank = Rank, Status = Status };
            await _categoryService.AddAsync(category);
        }
    }
}