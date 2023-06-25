using System;

namespace Test.Models
{
    [Serializable]
    public class ProductModel
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual decimal Price { get; set; }
        public virtual CategoryModel Category { get; set; }
    }
}