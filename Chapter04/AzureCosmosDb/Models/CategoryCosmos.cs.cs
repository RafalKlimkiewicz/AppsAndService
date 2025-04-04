namespace AzureCosmosDb.Models;

public class CategoryCosmos
{
#pragma warning disable IDE1006 // Naming Styles
    public int categoryId { get; set; }
    public string categoryName { get; set; } = null!;
    public string? description { get; set; }
#pragma warning restore IDE1006 // Naming Styles
}
