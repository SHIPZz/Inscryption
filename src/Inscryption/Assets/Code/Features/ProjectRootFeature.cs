using Code.Features.Game;
using Code.Features.Input;
using Code.Features.UI;
using Code.Infrastructure.Systems;

namespace Code.Features
{
    public class ProjectRootFeature : Feature
    {
        public ProjectRootFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<GameplayCoreFeature>());
            Add(systemFactory.Create<UIFeature>());
            Add(systemFactory.Create<GameStateFeature>());
            Add(systemFactory.Create<GameCleanupFeature>());
        }
    }
}

