using ConsoleApp1.Models;

using (var session = NHibernateHelper.GetSession())
{
    using (var tx = session.BeginTransaction())
    {

        //var product = new Product { Name = "Product06", Price = 120.65m };
        //var category = new Category { Name = "Category B" };
        //product.Category = category;
        //category.Products.Add(product);

        //session.Save(category);
        //tx.Commit();

        var c = session.Get<Category>(5);
        foreach (var item in c.Products)
            Console.WriteLine($"{item.Name}: {item.Price}");
    }

}
