using System.Text.Json.Serialization;

namespace WebAPITemplate.Models;

public class Product
{
    public Product()
    {
        // Deliberately set empty GUID
        Id = new Guid();
    }

    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal DeliveryPrice { get; set; }

    [JsonIgnore] public bool IsNew => Id.Equals(Guid.Empty);
}