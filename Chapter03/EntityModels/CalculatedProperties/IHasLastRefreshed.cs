namespace EntityModels.CalculatedProperties;

public interface IHasLastRefreshed
{
    DateTimeOffset LastRefreshDate { get; set; }
}
