namespace WebAPITemplate.Models;

public class ProductOptions
{
    public ProductOptions(List<ProductOption> items)
    {
        Items = items;
    }

    public List<ProductOption> Items { get; private set; }
}