using Code.Features.Game.Systems;
using Code.Infrastructure.Systems;

namespace Code.Features.Game
{
    public class GameStateFeature : Feature
    {
        public GameStateFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<InitializeGameSystem>());
            Add(systemFactory.Create<CheckVictorySystem>());
        }
    }
}

