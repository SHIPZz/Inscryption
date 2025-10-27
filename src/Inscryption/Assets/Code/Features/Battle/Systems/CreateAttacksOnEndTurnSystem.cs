using System.Collections.Generic;
using System.Linq;
using Code.Features.Board;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Battle.Systems
{
    public class CreateAttacksOnEndTurnSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _slots;
        private readonly List<GameEntity> _requestBuffer = new(1);
        private readonly List<GameEntity> _slotsBuffer = new(8);

        public CreateAttacksOnEndTurnSystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _game = game;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
            _endTurnRequests = game.GetGroup(GameMatcher.EndTurnRequest);
            _slots = game.GetGroup(GameMatcher.BoardSlot);
        }

        public void Execute()
        {
            foreach (GameEntity request in _endTurnRequests.GetEntities(_requestBuffer))
            {
                var (attacker, defender) = GetTurnPlayers();

                if (attacker != null)
                {
                    ProcessAttacks(attacker, defender);
                }
            }
        }

        private void ProcessAttacks(GameEntity attacker, GameEntity defender)
        {
            string attackerName = attacker.isHero ? "Hero" : "Enemy";
            Debug.Log($"[CreateAttacksOnEndTurnSystem] Creating attacks for {attackerName}'s cards");

            foreach (var slot in GetAttackerSlots(attacker))
            {
                if (!IsValidAttacker(slot, out var attackerCard))
                    continue;

                var target = FindAttackTarget(slot, defender);
                if (target != null)
                {
                    CreateAttackRequest(attackerCard, target, attackerCard.Damage, slot.SlotLane);
                }
            }
        }

        private (GameEntity attacker, GameEntity defender) GetTurnPlayers()
        {
            GameEntity hero = _heroProvider.GetHero();
            GameEntity enemy = _enemyProvider.GetEnemy();

            if (hero?.isHeroTurn == true)
                return (hero, enemy);

            if (enemy?.isEnemyTurn == true)
                return (enemy, hero);

            return (null, null);
        }

        private IEnumerable<GameEntity> GetAttackerSlots(GameEntity attacker)
        {
            return _slots.GetEntities(_slotsBuffer)
                .Where(s => s.hasSlotOwner && s.hasSlotLane && s.SlotOwner == attacker.Id)
                .OrderBy(s => s.SlotLane);
        }

        private bool IsValidAttacker(GameEntity slot, out GameEntity card)
        {
            card = null;

            if (!slot.isOccupied || slot.OccupiedBy < 0)
                return false;

            card = _game.GetEntityWithId(slot.OccupiedBy);
            return card != null && !card.isDestructed && card.hasDamage;
        }

        private GameEntity FindAttackTarget(GameEntity slot, GameEntity defender)
        {
            var oppositeSlot = BoardHelpers.FindOppositeSlot(_game, slot);

            if (oppositeSlot?.isOccupied == true && oppositeSlot.OccupiedBy >= 0)
            {
                var targetCard = _game.GetEntityWithId(oppositeSlot.OccupiedBy);
                if (targetCard != null && !targetCard.isDestructed)
                    return targetCard;
            }

            return defender;
        }

        private void CreateAttackRequest(GameEntity attacker, GameEntity target, int damage, int lane)
        {
            _game.CreateEntity().AddAttackRequest(attacker.Id, target.Id, damage);

            string targetType = target.isCard ? $"card {target.Id}" : $"player {target.Id}";
            Debug.Log($"[CreateAttacksOnEndTurnSystem] Card {attacker.Id} attacks {targetType} for {damage} damage (Lane {lane})");
        }
    }
}
