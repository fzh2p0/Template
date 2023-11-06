using Microsoft.EntityFrameworkCore;
using WebAPITemplate.AppData;

namespace WebAPITemplate.Models;

public class ProductOptionDataAccess
{
    private readonly ProductContext _context;

    public ProductOptionDataAccess(IServiceProvider serviceProvider)
    {
        _context = serviceProvider.GetRequiredService<ProductContext>();
    }

    /// <summary>
    ///     Gets a list of product options by product ID.
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>List of product options</returns>
    public IEnumerable<ProductOption> GetProductOptionsByProductId(Guid productId)
    {
        var productOptions = _context.ProductOption.Where(p => p.ProductId == productId);
        return productOptions;
    }

    /// <summary>
    ///     Gets a product option by ID.
    /// </summary>
    /// <param name="id">Product option ID</param>
    /// <returns>Product option object if found, an empty option if not found</returns>
    public ProductOption GetProductOptionById(Guid id)
    {
        var productOption = _context.ProductOption.FirstOrDefault(p => p.Id == id) ?? new ProductOption();
        return productOption;
    }

    /// <summary>
    ///     Deletes a product option by ID.
    /// </summary>
    /// <param name="id">Product option ID</param>
    /// <returns>True if the option was deleted, false if not found</returns>
    public bool Delete(Guid id)
    {
        var productOption = _context.ProductOption.FirstOrDefault(p => p.Id == id);
        if (productOption != null)
        {
            _context.ProductOption.Remove(productOption);
            _context.SaveChanges();
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Saves changes to a product option.
    /// </summary>
    /// <param name="productOption">Product option to save</param>
    /// <returns>The number of state entries written to the database</returns>
    public int Save(ProductOption productOption)
    {
        _context.Entry(productOption).State = EntityState.Modified;
        return _context.SaveChanges();
    }
}