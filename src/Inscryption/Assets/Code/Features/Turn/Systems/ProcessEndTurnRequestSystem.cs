using Entitas;
using System.Collections.Generic;

namespace Code.Features.Turn.Systems
{
    public class ProcessEndTurnRequestSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly List<GameEntity> _buffer = new(4);

        public ProcessEndTurnRequestSystem(GameContext game)
        {
            _endTurnRequests = game.GetGroup(GameMatcher.EndTurnRequest);
        }

        public void Execute()
        {
            foreach (GameEntity request in _endTurnRequests.GetEntities(_buffer))
            {
                request.Destroy();
            }
        }
    }
}

