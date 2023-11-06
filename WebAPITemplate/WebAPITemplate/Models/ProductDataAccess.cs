using Microsoft.EntityFrameworkCore;
using WebAPITemplate.AppData;

namespace WebAPITemplate.Models;

public class ProductDataAccess
{
    private readonly ProductContext _context;

    public ProductDataAccess(IServiceProvider serviceProvider)
    {
        _context = serviceProvider.GetRequiredService<ProductContext>();
    }

    /// <summary>
    ///     Gets a list of all products.
    /// </summary>
    /// <returns>List of products</returns>
    public Products GetAll()
    {
        return new Products(_context.Product.ToList());
    }

    /// <summary>
    ///     Gets a product by ID.
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product object if found, an empty product if not found</returns>
    public Product GetProductById(Guid id)
    {
        var product = _context.Product.FirstOrDefault(p => p.Id == id) ?? new Product();
        return product;
    }

    /// <summary>
    ///     Deletes a product by ID.
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>True if the product was deleted, false if not found</returns>
    public bool Delete(Guid id)
    {
        var product = _context.Product.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _context.Product.Remove(product);
            _context.SaveChanges();
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Gets products by name.
    /// </summary>
    /// <param name="name">Name to search for</param>
    /// <returns>Matching products</returns>
    public IEnumerable<Product> GetProductsByName(string name)
    {
        return _context.Product.Where(p => EF.Functions.Like(p.Name, "%" + name + "%"));
    }

    /// <summary>
    ///     Saves changes to a product.
    /// </summary>
    /// <param name="product">Product to save</param>
    /// <returns>The number of state entries written to the database</returns>
    public int Save(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        return _context.SaveChanges();
    }
}