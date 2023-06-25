using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models
{
    [Serializable]
    public class CategoryModel
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<ProductModel> Products { get; set; }

        public CategoryModel()
        {
            Products = new List<ProductModel>();
        }

    }
}