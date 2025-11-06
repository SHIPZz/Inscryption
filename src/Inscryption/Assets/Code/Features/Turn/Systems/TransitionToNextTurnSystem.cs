using Code.Common.Time;
using Code.Features.Turn.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class TransitionToNextTurnSystem : IExecuteSystem
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ITimerService _timerService;
        private readonly GameConfig _gameConfig;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private bool _transitioned;

        public TransitionToNextTurnSystem(
            GameContext game,
            IGameStateMachine gameStateMachine,
            ITimerService timerService,
            IConfigService configService)
        {
            _gameStateMachine = gameStateMachine;
            _timerService = timerService;
            _gameConfig = configService.GetConfig<GameConfig>();
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
        }

        public void Execute()
        {
            if (_transitioned)
                return;

            // Определяем текущего игрока ПЕРЕД очисткой флага
            GameEntity currentPlayer = TurnExtensions.GetCurrentPlayer(_heroes, _enemies);
            
            if (currentPlayer == null)
                return;

            // Определяем следующего игрока на основе текущего
            GameEntity nextPlayer = null;
            if (currentPlayer.isHero)
            {
                // Текущий - герой, следующий - враг
                foreach (GameEntity enemy in _enemies)
                {
                    nextPlayer = enemy;
                    break;
                }
            }
            else if (currentPlayer.isEnemy)
            {
                // Текущий - враг, следующий - герой
                foreach (GameEntity hero in _heroes)
                {
                    nextPlayer = hero;
                    break;
                }
            }

            if (nextPlayer == null)
                return;

            _transitioned = true;

            if (nextPlayer.isHero)
            {
                float delay = _gameConfig.AnimationTiming.EnemyTurnDelay + _gameConfig.AnimationTiming.PostAttackDelay;
                
                _timerService.Schedule(delay, () => {
                    _gameStateMachine.EnterAsync<States.HeroTurnState>().Forget();
                });
            }
            else if (nextPlayer.isEnemy)
            {
                _gameStateMachine.EnterAsync<States.EnemyTurnState>().Forget();
            }
        }
    }
}

