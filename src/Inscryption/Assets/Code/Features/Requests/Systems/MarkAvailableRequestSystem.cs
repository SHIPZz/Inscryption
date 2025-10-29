using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Requests.Systems
{
    public class MarkAvailableRequestSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new(16);

        public MarkAvailableRequestSystem(GameContext game)
        {
            _requests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Request));
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests.GetEntities(_buffer))
            {
                if (request.hasCooldown && !request.isCooldownUp)
                    request.isProcessingAvailable = false;
                else
                    request.isProcessingAvailable = true;
            }
        }
    }
}