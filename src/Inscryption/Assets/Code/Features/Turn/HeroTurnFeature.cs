using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class HeroTurnFeature : Feature
    {
        public HeroTurnFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<SetHeroTurnSystem>());
            Add(systemFactory.Create<TransitionToDrawSystem>());
        }
    }
}

