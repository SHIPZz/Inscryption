using Code.Common.Time;
using Code.Features.Turn.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class TransitionFromDrawSystem : IInitializeSystem
    {
        private readonly GameContext _game;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ITimerService _timerService;
        private readonly GameConfig _gameConfig;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;

        public TransitionFromDrawSystem(
            GameContext game,
            IGameStateMachine gameStateMachine,
            ITimerService timerService,
            IConfigService configService)
        {
            _game = game;
            _gameStateMachine = gameStateMachine;
            _timerService = timerService;
            _gameConfig = configService.GetConfig<GameConfig>();
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
        }

        public void Initialize()
        {
            GameEntity player = TurnExtensions.GetCurrentPlayer(_heroes, _enemies);
            if (player == null)
                return;

            int maxHandSize = _gameConfig.GameBalance.MaxHandSize;
            int cardsToDraw = UnityEngine.Mathf.Max(0, maxHandSize - player.CardsInHand.Count);

            if (cardsToDraw > 0)
            {
                float delay = _gameConfig.AnimationTiming.CardMoveDuration * cardsToDraw;
                _timerService.Schedule(delay, () => TransitionToNextState(player));
            }
            else
            {
                TransitionToNextState(player);
            }
        }

        private void TransitionToNextState(GameEntity player)
        {
            if (player.isHero)
            {
                _gameStateMachine.EnterAsync<States.PlacementState, int>(player.Id).Forget();
            }
            else if (player.isEnemy)
            {
                _gameStateMachine.EnterAsync<States.EnemyPlaceCardsState, int>(player.Id).Forget();
            }
        }
    }
}

