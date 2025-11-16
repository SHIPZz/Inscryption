using Code.Common.Time;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class TransitionToAttackAfterPlacementSystem : IInitializeSystem
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ITimerService _timerService;
        private readonly GameConfig _gameConfig;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _placeCardRequests;

        public TransitionToAttackAfterPlacementSystem(
            GameContext game,
            IGameStateMachine gameStateMachine,
            ITimerService timerService,
            IConfigService configService)
        {
            _gameStateMachine = gameStateMachine;
            _timerService = timerService;
            _gameConfig = configService.GetConfig<GameConfig>();
            _enemies = game.GetGroup(GameMatcher.Enemy);
            _placeCardRequests = game.GetGroup(GameMatcher.PlaceCardRequest);
        }

        public void Initialize()
        {
            foreach (GameEntity enemy in _enemies)
            {
                if (!enemy.isEnemyTurn)
                    continue;

                // Ждем, пока запрос на размещение обработается
                if (_placeCardRequests.count > 0)
                    return;

                // Переходим к атаке после размещения карты
                float delay = _gameConfig.AnimationTiming.CardMoveDuration + _gameConfig.AnimationTiming.PostAttackDelay;
                _timerService.Schedule(delay, () =>
                {
                    _gameStateMachine.EnterAsync<States.AttackState, int>(enemy.Id).Forget();
                });
                return;
            }
        }
    }
}

