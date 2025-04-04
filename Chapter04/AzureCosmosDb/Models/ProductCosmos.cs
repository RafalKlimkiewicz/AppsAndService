namespace AzureCosmosDb.Models;

public class ProductCosmos
{
#pragma warning disable IDE1006 // Naming Styles
    public string id { get; set; } = null!;
    public string productId { get; set; } = null!;
    public string productName { get; set; } = null!;
    public string? quantityPerUnit { get; set; }
    public decimal? unitPrice { get; set; }
    public short? unitsInStock { get; set; }
    public short? unitsOnOrder { get; set; }
    public short? reorderLevel { get; set; }
    public bool discontinued { get; set; }
    public CategoryCosmos? category { get; set; }
    public SupplierCosmos? supplier { get; set; }
#pragma warning restore IDE1006 // Naming Styles
}
