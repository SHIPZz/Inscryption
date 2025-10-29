using System.Collections.Generic;
using System.Linq;
using Code.Features.Board;
using Entitas;
using UnityEngine;

namespace Code.Features.Battle.Systems
{
    public class AnimateOnAttackSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _slots;

        public AnimateOnAttackSystem(GameContext game) : base(game)
        {
            _game = game;
            _slots = _game.GetGroup(GameMatcher.BoardSlot);
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AttackRequest.Added(), GameMatcher.ProcessingAvailable.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasAttackRequest && entity.isProcessingAvailable;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (GameEntity attackRequest in entities)
            {
                GameEntity attacker = _game.GetEntityWithId(attackRequest.attackRequest.AttackerId);
                GameEntity target = _game.GetEntityWithId(attackRequest.attackRequest.TargetId);

                if (attacker == null || target == null || !attacker.hasAttackAnimator)
                    continue;

                if (TryGetTargetTransform(attacker, target, out Transform targetTransform))
                {
                    attacker.AttackAnimator.PlayAttackAnimation(targetTransform);
                }
            }
        }

        private bool TryGetTargetTransform(GameEntity attacker, GameEntity target, out Transform targetTransform)
        {
            targetTransform = null;

            if (target.hasView)
            {
                targetTransform = target.Transform;
                return targetTransform != null;
            }

            GameEntity attackerSlot = _slots.GetEntities()
                .FirstOrDefault(s => s.isOccupied && s.OccupiedBy == attacker.Id);

            if (attackerSlot == null)
                return false;

            GameEntity oppositeSlot = BoardHelpers.FindOppositeSlot(_game, attackerSlot);
       
            if (oppositeSlot != null && oppositeSlot.hasView)
            {
                targetTransform = oppositeSlot.Transform;
                return targetTransform != null;
            }

            return false;
        }
    }
}