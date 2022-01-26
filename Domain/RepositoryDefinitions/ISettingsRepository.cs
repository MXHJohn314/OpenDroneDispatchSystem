using System.Threading.Tasks;
using DatabaseAccess;
using Domain.DTO;

namespace Domain.RepositoryDefinitions;

public interface ISettingsRepository
{
    public Task<GeoLocation> GetHomeLocation();

}