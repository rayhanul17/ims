using IMS.BusinessRules.Enum;

namespace IMS.BusinessModel.ViewModel
{
    public class CategoryAddModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }
        public Status Status { get; set; }

        //private ICategoryService _categoryService;

        //public CategoryCreateModel()
        //{

        //}

        //public CategoryCreateModel(ICategoryService categoryService)
        //{
        //    _categoryService = categoryService;
        //}

        //public void ResolveDependency(ILifetimeScope scope)
        //{
        //    _scope = scope;
        //    _categoryService = _scope.Resolve<ICategoryService>();
        //}

        //internal async Task AddAsync()
        //{
        //    var category = new CategoryAdd { Name = Name, Description = Description, Rank = Rank, Status = Status };
        //    await _categoryService.AddAsync(category);
        //}
    }
}
