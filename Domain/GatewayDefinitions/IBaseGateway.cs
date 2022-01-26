using System.Threading.Tasks;

namespace Domain.GatewayDefinitions;

public interface IBaseGateway<T1>
{
    public Task<TResponse> SendMessagePost<TRequest, TResponse>(string url, TRequest requestDto,
        bool isDebug = true);
}