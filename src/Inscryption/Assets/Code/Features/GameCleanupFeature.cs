using Code.Common.Destruct;
using Code.Infrastructure.Systems;

namespace Code.Features
{
    public sealed class GameCleanupFeature : Feature
    {
        public GameCleanupFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<CleanupGameRequestsSystem>());
            Add(systemFactory.Create<CleanupGameDestructedViewSystem>());
            Add(systemFactory.Create<CleanupMetaDestructedSystem>());
            Add(systemFactory.Create<CleanupGameDestructedSystem>());
        }
    }
}