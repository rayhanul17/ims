using ConsoleApp1.Models;

using (var session = NHibernateHelper.GetSession())
{
    using (var tx = session.BeginTransaction())
    {

        var product = new Product { Name = "Rajaaaaaaaaaaaa", Price = 120.65m };
        var category = new Category { Name = "Category User02", Products = new List<Product> { product } };
        //product.Category = category;
        

        session.Save(category);
        tx.Commit();

        //var c = session.Get<Category>(5);
        //foreach (var item in c.Products)
        //    Console.WriteLine($"{item.Name}: {item.Price}");
    }

}
