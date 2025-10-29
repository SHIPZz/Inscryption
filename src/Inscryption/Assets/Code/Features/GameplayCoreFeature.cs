using Code.Common.Destruct;
using Code.Features.Battle;
using Code.Features.Board;
using Code.Features.Camera;
using Code.Features.Cards;
using Code.Features.Cheats;
using Code.Features.Cooldowns;
using Code.Features.Enemy;
using Code.Features.Hero;
using Code.Features.Movement;
using Code.Features.Requests;
using Code.Features.Stats;
using Code.Features.Statuses;
using Code.Features.Turn;
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
            Add(systemFactory.Create<RequestFeature>());
            Add(systemFactory.Create<MovementFeature>());
            Add(systemFactory.Create<CardFeature>());
            Add(systemFactory.Create<HeroFeature>());
            Add(systemFactory.Create<EnemyFeature>());
            Add(systemFactory.Create<BoardFeature>());
            Add(systemFactory.Create<BattleFeature>());
            Add(systemFactory.Create<TurnFeature>());

            Add(systemFactory.Create<StatusFeature>());
            Add(systemFactory.Create<StatsFeature>());
            Add(systemFactory.Create<ProcessDestructedFeature>());
        }
    }
}