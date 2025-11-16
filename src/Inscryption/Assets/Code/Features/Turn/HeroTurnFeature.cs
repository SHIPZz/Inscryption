using Code.Features.Cards;
using Code.Features.Input;
using Code.Features.Turn.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Turn
{
    public class HeroTurnFeature : Feature
    {
        public HeroTurnFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<SetHeroTurnSystem>());
            Add(systemFactory.Create<InputFeature>());
            Add(systemFactory.Create<CardFeature>());
            Add(systemFactory.Create<TransitionToDrawSystem>());
        }
    }
}

