using Code.Features.Requests.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Requests
{
    public class RequestFeature : Feature
    {
        public RequestFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<MarkAvailableRequestSystem>());
        }
    }
}