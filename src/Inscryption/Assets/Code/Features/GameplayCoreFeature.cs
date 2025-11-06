using Code.Common.Destruct;
using Code.Features.Camera;
using Code.Features.Cheats;
using Code.Features.Cooldowns;
using Code.Features.Death;
using Code.Features.Movement;
using Code.Features.Stats;
using Code.Features.Statuses;
using Code.Features.View;
using Code.Infrastructure.Systems;

namespace Code.Features
{
    public sealed class GameplayCoreFeature : Feature
    {
        public GameplayCoreFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<BindViewFeature>());
            Add(systemFactory.Create<CameraFeature>());
            Add(systemFactory.Create<CooldownFeature>());
            Add(systemFactory.Create<CheatFeature>());
            Add(systemFactory.Create<MovementFeature>());

            Add(systemFactory.Create<StatusFeature>());
            Add(systemFactory.Create<StatsFeature>());
            Add(systemFactory.Create<DeathFeature>());
            Add(systemFactory.Create<ProcessDestructedFeature>());
        }
    }
}