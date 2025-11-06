using Code.Common.Time;
using Code.Features.Board.Extensions;
using Code.Features.Turn.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class TransitionFromAttackSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ITimerService _timerService;
        private readonly GameConfig _gameConfig;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _slots;
        private readonly IGroup<GameEntity> _attackRequests;
        private bool _transitionScheduled;

        public TransitionFromAttackSystem(
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
            _slots = game.GetGroup(GameMatcher.BoardSlot);
            _attackRequests = game.GetGroup(GameMatcher.AttackRequest);
        }

        public void Execute()
        {
            if (_transitionScheduled)
                return;

            // Ждем, пока все атаки завершатся
            if (_attackRequests.count > 0)
                return;

            var (attacker, defender) = TurnExtensions.GetBattleParticipants(_heroes, _enemies);
            if (attacker == null)
                return;

            _transitionScheduled = true;
            // Задержка после атаки + время возврата карты (0.5 delay + 0.3 duration = 0.8)
            float cardReturnTime = 0.8f;
            float delay = _gameConfig.AnimationTiming.PostAttackDelay + cardReturnTime;

            _timerService.Schedule(delay, () =>
            {
                _gameStateMachine.EnterAsync<States.SwitchTurnState>().Forget();
            });
        }
    }
}

