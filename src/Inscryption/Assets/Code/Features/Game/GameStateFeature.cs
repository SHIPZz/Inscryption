using Code.Features.Game.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Game
{
    public class GameStateFeature : Feature
    {
        public GameStateFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<CheckVictorySystem>());
            Add(systemFactory.Create<ProcessGameEndRequestSystem>());
        }
    }
}

