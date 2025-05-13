using System.Threading.Tasks;

namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IApiClient
{
    Task<TResponse> Get<TResponse>(IGetApiRequest request);
    Task<int> Ping();
    Task<TResponse> Post<TResponse, TPostData>(IPostApiRequest<TPostData> request);
    Task<TResponse> Delete<TResponse>(IDeleteApiRequest request);
}
