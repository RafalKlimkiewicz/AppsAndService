﻿using System.Text.Json;

using DataContext;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

using Northwind.EntityModels;

namespace Northwind.WebApi.Service.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private int pageSize = 10;

    private readonly ILogger<ProductsController> _logger;
    private readonly NorthwindContext _db;
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;

    private const string OutOfStockProductsKey = "OOSP";

    private const string DiscontinuedProductsKey = "DISCP";

    public ProductsController(ILogger<ProductsController> logger,
        NorthwindContext db,
        IMemoryCache memoryCache,
        IDistributedCache distributedCache)
    {
        _logger = logger;
        _db = db;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
    }

    // GET: api/products
    [HttpGet]
    [Produces(typeof(Product[]))]
    public IEnumerable<Product> Get(int? page)
    {
        return _db.Products
             .Where(p => p.UnitsInStock > 0 && !p.Discontinued)
             .OrderBy(product => product.ProductId)
             .Skip(((page ?? 1) - 1) * pageSize)
             .Take(pageSize);
    }

    // GET: api/products/outofstock
    [HttpGet]
    [Route("outofstock")]
    [Produces(typeof(Product[]))]
    public IEnumerable<Product> GetOutOfStockProducts()
    {
        if (!_memoryCache.TryGetValue(OutOfStockProductsKey, out Product[]? cachedValue))
        {
            cachedValue = _db.Products.Where(p => p.UnitsInStock == 0 && !p.Discontinued).ToArray();

            MemoryCacheEntryOptions cacheEntryOptions = new()
            {
                SlidingExpiration = TimeSpan.FromSeconds(5),
                Size = cachedValue?.Length
            };

            _memoryCache.Set(OutOfStockProductsKey, cachedValue, cacheEntryOptions);
        }

        MemoryCacheStatistics? stats = _memoryCache.GetCurrentStatistics();

        _logger.LogInformation($"Memory cache. Total hits: {stats?.TotalHits}. Estimated size: {stats?.CurrentEstimatedSize}.");

        return cachedValue ?? Enumerable.Empty<Product>();
    }

    // GET: api/products/discontinued
    [HttpGet]
    [Route("discontinued")]
    [Produces(typeof(Product[]))]
    public IEnumerable<Product> GetDiscontinuedProducts()
    {
        // Try to get the cached value.
        byte[]? cachedValueBytes = _distributedCache.Get(DiscontinuedProductsKey);
        Product[]? cachedValue = null;

        if (cachedValueBytes is null)
        {
            cachedValue = GetDiscontinuedProductsFromDatabase();
        }
        else
        {
            cachedValue = JsonSerializer.Deserialize<Product[]?>(cachedValueBytes);

            cachedValue ??= GetDiscontinuedProductsFromDatabase();
        }

        return cachedValue ?? Enumerable.Empty<Product>();
    }

    // GET api/products/5
    [HttpGet("{id:int}")]
    [ResponseCache(Duration = 5, // Cache-Control: max-age=5
    Location = ResponseCacheLocation.Any, // Cache-Control: public
    VaryByHeader = "User-Agent" // Vary: User-Agent
    )]
    public async ValueTask<Product?> Get(int id)
    {
        return await _db.Products.FindAsync(id);
    }

    // GET api/products/cha
    [HttpGet("{name}")]
    public IEnumerable<Product> Get(string name)
    {
        if (Random.Shared.Next(1, 4) == 1)
        {
            return _db.Products.Where(p => p.ProductName.Contains(name));
        }

        throw new Exception("Randomized fault.");
    }

    // POST api/products
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Product product)
    {
        _db.Products.Add(product);

        await _db.SaveChangesAsync();

        return Created($"api/products/{product.ProductId}", product);
    }

    // PUT api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Product product)
    {
        Product? foundProduct = await _db.Products.FindAsync(id);

        if (foundProduct is null)
            return NotFound();

        foundProduct.ProductName = product.ProductName;
        foundProduct.CategoryId = product.CategoryId;
        foundProduct.SupplierId = product.SupplierId;
        foundProduct.QuantityPerUnit = product.QuantityPerUnit;
        foundProduct.UnitsInStock = product.UnitsInStock;
        foundProduct.UnitsOnOrder = product.UnitsOnOrder;
        foundProduct.ReorderLevel = product.ReorderLevel;
        foundProduct.UnitPrice = product.UnitPrice;
        foundProduct.Discontinued = product.Discontinued;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _db.Products.FindAsync(id) is Product product)
        {
            _db.Products.Remove(product);

            await _db.SaveChangesAsync();

            return NoContent();
        }
        return NotFound();
    }

    private Product[]? GetDiscontinuedProductsFromDatabase()
    {
        Product[]? cachedValue = _db.Products.Where(product => product.Discontinued).ToArray();

        DistributedCacheEntryOptions cacheEntryOptions = new()
        {
            // Allow readers to reset the cache entry's lifetime.
            SlidingExpiration = TimeSpan.FromSeconds(5),
            // Set an absolute expiration time for the cache entry.
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20),
        };

        byte[]? cachedValueBytes = JsonSerializer.SerializeToUtf8Bytes(cachedValue);

        _distributedCache.Set(DiscontinuedProductsKey, cachedValueBytes, cacheEntryOptions);

        return cachedValue;
    }
}
