using System.Collections.Generic;
using Code.Features.Statuses.Services;
using Entitas;

namespace Code.Features.Battle.Systems
{
    public class AnimateOnAttackSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _game;

        public AnimateOnAttackSystem(GameContext game) : base(game)
        {
            _game = game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AttackRequest.Added(),GameMatcher.ProcessingAvailable.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasAttackRequest && entity.isProcessingAvailable;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (GameEntity attackRequest in entities)
            {
                int attackerId = attackRequest.attackRequest.AttackerId;
                int targetId = attackRequest.attackRequest.TargetId;
                GameEntity attacker = _game.GetEntityWithId(attackerId);
                GameEntity target = _game.GetEntityWithId(targetId);

                if (attacker == null || target == null)
                    continue;

                if (!attacker.hasCardAnimator)
                    continue;

                if (attacker.hasAttackAnimator)
                    attacker.AttackAnimator.PlayAttackAnimation(targetId);
            }
        }
    }
}