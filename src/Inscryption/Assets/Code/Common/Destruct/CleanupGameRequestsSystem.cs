using System.Collections.Generic;
using Entitas;

namespace Code.Common.Destruct
{
    public class CleanupGameRequestsSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new(32);

        public CleanupGameRequestsSystem(GameContext game)
        {
            _requests = game.GetGroup(GameMatcher.Request);
        }

        public void Cleanup()
        {
            foreach (GameEntity request in _requests.GetEntities(_buffer))
            {
                request.Destroy();
            }
        }
    }
}