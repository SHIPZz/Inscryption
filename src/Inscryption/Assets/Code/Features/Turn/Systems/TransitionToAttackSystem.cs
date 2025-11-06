using Code.Features.Turn.Extensions;
using Code.Features.Turn.StateMachine;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class TransitionToAttackSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _heroes;

        public TransitionToAttackSystem(GameContext game, IGameStateMachine gameStateMachine)
        {
            _game = game;
            _gameStateMachine = gameStateMachine;
            _endTurnRequests = game.GetGroup(GameMatcher.EndTurnRequest);
            _heroes = game.GetGroup(GameMatcher.Hero);
        }

        public void Execute()
        {
            if (_endTurnRequests.count == 0)
                return;

            GameEntity hero = TurnExtensions.GetCurrentPlayer(_heroes, null);
            if (hero == null || !hero.isHeroTurn)
                return;

            _gameStateMachine.EnterAsync<States.AttackState, int>(hero.Id).Forget();
        }
    }
}

