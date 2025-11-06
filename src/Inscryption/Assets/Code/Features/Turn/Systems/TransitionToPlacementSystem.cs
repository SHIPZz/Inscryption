using Code.Features.Turn.StateMachine;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class TransitionToPlacementSystem : IInitializeSystem
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IGroup<GameEntity> _heroes;

        public TransitionToPlacementSystem(GameContext game, IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
            _heroes = game.GetGroup(GameMatcher.Hero);
        }

        public void Initialize()
        {
            foreach (GameEntity hero in _heroes)
            {
                if (hero.isHeroTurn)
                {
                    _gameStateMachine.EnterAsync<States.PlacementState, int>(hero.Id).Forget();
                }
            }
        }
    }
}

