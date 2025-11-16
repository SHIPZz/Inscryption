using System.Collections.Generic;
using Code.Common.Time;
using Entitas;

namespace Code.Features.Cooldowns.Systems
{
    public class CalculateCooldownSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _cooldowns;
        private readonly ITimeService _timeService;
        private readonly List<GameEntity> _buffer = new(16);

        public CalculateCooldownSystem(GameContext game, ITimeService timeService)
        {
            _timeService = timeService;
            _cooldowns = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Cooldown));
        }

        public void Execute()
        {
            foreach (GameEntity cooldown in _cooldowns.GetEntities(_buffer))
            {
                cooldown.ReplaceCooldownLeft(cooldown.CooldownLeft - _timeService.DeltaTime);

                if (cooldown.CooldownLeft <= 0)
                {
                    cooldown.ReplaceCooldownLeft(0);
                    cooldown.isCooldownUp = true;
                    cooldown.RemoveCooldown();
                }
            }
        }
    }
}