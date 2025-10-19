using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class TurnFeature : Feature
    {
        public TurnFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<ProcessEndTurnRequestSystem>());
            Add(systemFactory.Create<ProcessSwitchTurnRequestSystem>());
        }
    }
}

