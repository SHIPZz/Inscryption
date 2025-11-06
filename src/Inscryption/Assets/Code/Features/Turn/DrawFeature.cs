using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class DrawFeature : Feature
    {
        public DrawFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<CreateDrawCardRequestsSystem>());
            Add(systemFactory.Create<TransitionFromDrawSystem>());
        }
    }
}

