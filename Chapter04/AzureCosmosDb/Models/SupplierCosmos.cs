namespace AzureCosmosDb.Models;

public class SupplierCosmos
{
#pragma warning disable IDE1006 // Naming Styles
    public int supplierId { get; set; }
    public string companyName { get; set; } = null!;
    public string? contactName { get; set; }
    public string? contactTitle { get; set; }
    public string? address { get; set; }
    public string? city { get; set; }
    public string? region { get; set; }
    public string? postalCode { get; set; }
    public string? country { get; set; }
    public string? phone { get; set; }
    public string? fax { get; set; }
    public string? homePage { get; set; }
#pragma warning restore IDE1006 // Naming Styles
}
