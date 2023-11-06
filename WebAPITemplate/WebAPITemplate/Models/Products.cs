namespace WebAPITemplate.Models;

public class Products
{
    public Products(List<Product> items)
    {
        Items = items;
    }

    public List<Product> Items { get; private set; }
}