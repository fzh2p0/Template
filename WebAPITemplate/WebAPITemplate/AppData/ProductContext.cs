using Microsoft.EntityFrameworkCore;
using WebAPITemplate.Models;

namespace WebAPITemplate.AppData
{
    /// <summary>
    /// Represents the database context for products and product options.
    /// </summary>
    public class ProductContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductContext"/> class.
        /// </summary>
        /// <param name="options">The options for configuring the database context.</param>
        /// <param name="logger">The logger for capturing log messages.</param>
        public ProductContext(
            DbContextOptions<ProductContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DbSet for products.
        /// </summary>
        public virtual DbSet<Product> Product { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for product options.
        /// </summary>
        public virtual DbSet<ProductOption> ProductOption { get; set; }
    }
}