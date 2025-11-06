using Code.Common;
using Code.Common.Extensions;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class TransitionToNextTurnSystem : IInitializeSystem
    {
        public TransitionToNextTurnSystem(GameContext game)
        {
        }

        public void Initialize()
        {
            CreateEntity.Request()
                .With(x => x.isSwitchTurnRequest = true);
        }
    }
}

