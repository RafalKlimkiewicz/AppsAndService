using System.ComponentModel.DataAnnotations.Schema;
using EntityModels.CalculatedProperties;

namespace Northwind.EntityModels;

public partial class Employee : IHasLastRefreshed
{
    [NotMapped]
    public DateTimeOffset LastRefreshDate { get; set; }
}
