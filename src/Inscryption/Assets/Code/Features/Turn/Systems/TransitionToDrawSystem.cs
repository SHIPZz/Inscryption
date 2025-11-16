using Code.Features.Turn.Extensions;
using Code.Features.Turn.StateMachine;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class TransitionToDrawSystem : IInitializeSystem
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;

        public TransitionToDrawSystem(GameContext game, IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
        }

        public void Initialize()
        {
            GameEntity player = TurnExtensions.GetCurrentPlayer(_heroes, _enemies);
            
            if (player == null)
                return;

            UnityEngine.Debug.Log($"[TransitionToDrawSystem] Transitioning to DrawState for {(player.isHero ? "hero" : "enemy")} {player.Id}");
            _gameStateMachine.EnterAsync<States.DrawState, int>(player.Id).Forget();
        }
    }
}

