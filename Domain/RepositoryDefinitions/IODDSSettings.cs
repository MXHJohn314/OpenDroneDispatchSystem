using Domain.DTO;

namespace Domain.RepositoryDefinitions;

public interface IODDSSettings
{
    public IFleetRepository GetFleetCollection();

    public IOrdersRepository GetOrdersCollection();
    string? GetAPIKey();
    GeoLocation GetHomeLocation();
    string GetDispatchUrl();
}