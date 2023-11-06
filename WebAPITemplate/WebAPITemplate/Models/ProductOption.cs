using System.Text.Json.Serialization;

namespace WebAPITemplate.Models;

public class ProductOption
{
    public ProductOption()
    {
        // Deliberately set empty GUID
        Id = new Guid();
    }

    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    [JsonIgnore] public bool IsNew => Id.Equals(Guid.Empty);
}