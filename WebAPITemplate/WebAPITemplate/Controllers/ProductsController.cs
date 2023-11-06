using System.Text;
using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using WebAPITemplate.Models;

namespace WebAPITemplate.Controllers;

/// <summary>
///     Provides CRUD operations for products and product options.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly ProductDataAccess _productDataAccess;
    private readonly ProductOptionDataAccess _productOptionsDataAccess;
    private readonly IValidator<ProductOption> _productOptionValidator;
    private readonly IValidator<Product> _productValidator;

    /// <summary>
    ///     Constructor that injects dependencies.
    /// </summary>
    public ProductsController(ProductDataAccess productDataAccess, ProductOptionDataAccess productOptionsDataAccess,
        IValidator<Product> productValidator, IValidator<ProductOption> productOptionValidator,
        ILogger<ProductsController> logger)
    {
        _logger = logger;
        _productDataAccess = productDataAccess;
        _productOptionsDataAccess = productOptionsDataAccess;
        _productValidator = productValidator;
        _productOptionValidator = productOptionValidator;
    }

    /// <summary>
    ///     Gets a list of all products.
    /// </summary>
    /// <returns>List of products</returns>
    [HttpGet]
    public Products GetAll()
    {
        return _productDataAccess.GetAll();
    }

    /// <summary>
    ///     Searches products by name.
    /// </summary>
    /// <param name="name">Name to search for</param>
    /// <returns>Matching products</returns>
    [HttpGet("{name}")]
    public Products SearchByName(string name)
    {
        var products = _productDataAccess.GetProductsByName(name);
        return new Products(products.ToList());
    }

    /// <summary>
    ///     Gets a product by ID.
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product object if found, 404 Not Found otherwise</returns>
    [HttpGet("{id:Guid}")]
    public IActionResult GetProduct(Guid id)
    {
        var product = _productDataAccess.GetProductById(id);

        if (product.IsNew)
        {
            _logger.LogWarning($"Product with id {id} not found");
            var errorResponse = new
            {
                StatusCode = NotFound(),
                Message = $"Product with id {id} not found"
            };
            return NotFound(errorResponse);
        }

        return Ok(product);
    }

    /// <summary>
    ///     Creates a new product.
    /// </summary>
    /// <param name="product">Product details</param>
    /// <returns>Created product object</returns>
    [HttpPost]
    public IActionResult Create(Product product)
    {
        var validationResult = _productValidator.Validate(product);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Product data invalid");
            return BadRequest(FormatValidationErrors(validationResult.Errors));
        }

        _productDataAccess.Save(product);
        return Ok(product);
    }

    /// <summary>
    ///     Updates a product.
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="product">Product details</param>
    /// <returns>Updated product object</returns>
    [HttpPut("{id:Guid}")]
    public IActionResult Update(Guid id, Product product)
    {
        var validationResult = _productValidator.Validate(product);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Product data invalid");
            return BadRequest(FormatValidationErrors(validationResult.Errors));
        }

        var storedProduct = _productDataAccess.GetProductById(id);
        if (storedProduct.IsNew)
        {
            _logger.LogWarning($"Product with id {id} not found");
            var errorResponse = new
            {
                StatusCode = NotFound(),
                Message = $"Product with id {id} not found"
            };
            return NotFound(errorResponse);
        }

        storedProduct.Name = product.Name;
        storedProduct.Description = product.Description;
        storedProduct.Price = product.Price;
        storedProduct.DeliveryPrice = product.DeliveryPrice;

        _productDataAccess.Save(storedProduct);
        return Ok(storedProduct);
    }

    private string FormatValidationErrors(List<ValidationFailure> validationResultErrors)
    {
        var sb = new StringBuilder();
        foreach (var validationResultError in validationResultErrors)
        {
            var errorResponse = new
            {
                validationResultError.PropertyName,
                validationResultError.ErrorMessage,
                validationResultError.AttemptedValue
            };

            sb.Append(JsonSerializer.Serialize(errorResponse));
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Deletes a product.
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>200 OK if deleted, 404 Not Found if not found</returns>
    [HttpDelete("{id:Guid}")]
    public IActionResult Delete(Guid id)
    {
        if (!_productDataAccess.Delete(id))
        {
            _logger.LogWarning($"Product with id {id} not found");
            var errorResponse = new
            {
                StatusCode = NotFound(),
                Message = $"Product with id {id} not found"
            };
            return NotFound(errorResponse);
        }

        return Ok();
    }

    /// <summary>
    ///     Gets options for a product.
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <returns>List of product options</returns>
    [HttpGet("{productId:Guid}/options")]
    public ProductOptions GetOptions(Guid productId)
    {
        var options = _productOptionsDataAccess.GetProductOptionsByProductId(productId);
        return new ProductOptions(options.ToList());
    }

    /// <summary>
    ///     Gets an option for a product by ID.
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="id">Option ID</param>
    /// <returns>Option object if found, 404 Not Found otherwise</returns>
    [HttpGet("{productId:Guid}/options/{id:Guid}")]
    public IActionResult GetOption(Guid productId, Guid id)
    {
        var option = _productOptionsDataAccess.GetProductOptionById(id);
        if (option.IsNew)
        {
            _logger.LogWarning($"Option with id {id} not found");
            var errorResponse = new
            {
                StatusCode = NotFound(),
                Message = $"Option with id {id} not found"
            };
            return NotFound(errorResponse);
        }

        return Ok(option);
    }

    /// <summary>
    ///     Creates an option for a product.
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="option">Option details</param>
    /// <returns>Created option object</returns>
    [HttpPost("{productId:Guid}/options")]
    public IActionResult CreateOption(Guid productId, ProductOption option)
    {
        var validationResult = _productOptionValidator.Validate(option);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Option data invalid");
            return BadRequest(FormatValidationErrors(validationResult.Errors));
        }

        option.ProductId = productId;
        _productOptionsDataAccess.Save(option);
        return Ok(option);
    }

    /// <summary>
    ///     Updates an option for a product.
    /// </summary>
    /// <param name="id">Option ID</param>
    /// <param name="option">Option details</param>
    /// <returns>Updated option object</returns>
    [HttpPut("{productId:Guid}/options/{id:Guid}")]
    public IActionResult UpdateOption(Guid id, ProductOption option)
    {
        var validationResult = _productOptionValidator.Validate(option);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Option data invalid");
            return BadRequest(FormatValidationErrors(validationResult.Errors));
        }

        var storedOption = _productOptionsDataAccess.GetProductOptionById(id);
        if (option.IsNew)
        {
            _logger.LogWarning($"Option with id {id} not found");
            var errorResponse = new
            {
                StatusCode = NotFound(),
                Message = $"Option with id {id} not found"
            };
            return NotFound(errorResponse);
        }

        storedOption.Name = option.Name;
        storedOption.Description = option.Description;

        _productOptionsDataAccess.Save(storedOption);
        return Ok(storedOption);
    }

    /// <summary>
    ///     Deletes an option for a product.
    /// </summary>
    /// <param name="id">Option ID</param>
    /// <returns>200 OK if deleted, 404 Not Found if not found</returns>
    [HttpDelete("{productId:Guid}/options/{id:Guid}")]
    public IActionResult DeleteOption(Guid id)
    {
        if (!_productOptionsDataAccess.Delete(id))
        {
            _logger.LogWarning($"Option with id {id} not found");
            var errorResponse = new
            {
                StatusCode = NotFound(),
                Message = $"Option with id {id} not found"
            };
            return NotFound(errorResponse);
        }

        return Ok();
    }
}