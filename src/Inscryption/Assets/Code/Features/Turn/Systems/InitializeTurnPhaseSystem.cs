using Entitas;

namespace Code.Features.Turn.Systems
{
    public class InitializeTurnPhaseSystem : IInitializeSystem
    {
        private readonly GameContext _game;

        public InitializeTurnPhaseSystem(GameContext game)
        {
            _game = game;
        }

        public void Initialize()
        {
            var phaseEntity = _game.CreateEntity();
            phaseEntity.AddCurrentTurnPhase(TurnPhase.PlayerPlacement);
        }
    }
}
