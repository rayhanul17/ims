namespace ConsoleApp1.Models;

public class Category
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual IList<Product> Products { get; set; }

    public Category()
    {
        Products = new List<Product>();
    }

}