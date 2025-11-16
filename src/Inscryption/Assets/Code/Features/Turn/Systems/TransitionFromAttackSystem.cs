using System.Collections.Generic;
using Code.Common.Time;
using Code.Features.Turn.Extensions;
using Code.Features.Turn.StateMachine;
using Code.Features.Turn.States;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class TransitionFromAttackSystem : IExecuteSystem
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ITimerService _timerService;
        private readonly GameConfig _gameConfig;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _attackRequests;
        private readonly IGroup<GameEntity> _allAttacksComplete;
        private readonly List<GameEntity> _buffer = new(4);
        private bool _transitionScheduled;

        public TransitionFromAttackSystem(
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
            game.GetGroup(GameMatcher.BoardSlot);
            _attackRequests = game.GetGroup(GameMatcher.AttackRequest);
            _allAttacksComplete = game.GetGroup(GameMatcher.AllAttacksComplete);
        }

        public void Execute()
        {
            if (_transitionScheduled)
                return;

            if (_attackRequests.count > 0)
                return;

            if (_allAttacksComplete.count == 0)
                return;

            var (attacker, defender) = TurnExtensions.GetBattleParticipants(_heroes, _enemies);
            if (attacker == null)
                return;

            _transitionScheduled = true;
            
            foreach (GameEntity completeFlag in _allAttacksComplete.GetEntities(_buffer))
            {
                completeFlag.Destroy();
            }
            
            float cardReturnTime = 0.8f;
            float delay = _gameConfig.AnimationTiming.PostAttackDelay + cardReturnTime;

            _timerService.Schedule(delay, () =>
            {
                _transitionScheduled = false;
                _gameStateMachine.EnterAsync<SwitchTurnState>().Forget();
            });
        }
    }
}

