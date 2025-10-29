using System.Collections.Generic;
using System.Linq;
using Code.Common;
using Code.Common.Extensions;
using Code.Features.Board;
using Code.Features.Cooldowns.Extensions;
using Code.Features.Turn;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Battle.Systems
{
    public class CreateAttacksOnEndTurnSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _slots;
        private readonly GameConfig _gameConfig;

        public CreateAttacksOnEndTurnSystem(GameContext game, IConfigService configService)
        {
            _game = game;
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
            _endTurnRequests = game.GetGroup(GameMatcher.AllOf(GameMatcher.EndTurnRequest, GameMatcher.ProcessingAvailable));
            _slots = game.GetGroup(GameMatcher.BoardSlot);
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public void Execute()
        {
            foreach (GameEntity request in _endTurnRequests)
            {
                (GameEntity attacker, GameEntity defender) = GetAttackerAndDefender();

                if (attacker != null)
                {
                    ProcessAttacks(attacker, defender);
                }
            }
        }

        private void ProcessAttacks(GameEntity attacker, GameEntity defender)
        {
            string attackerName = attacker.isHero ? "Hero" : "Enemy";
            Debug.Log($"[CreateAttacksOnEndTurnSystem] Creating attack queue for {attackerName}'s cards");

            float delay = 0f;

            IEnumerable<GameEntity> attackerSlots = GetAttackerSlots(attacker);
            
            for (int i = 0; i < attackerSlots.Count(); i++)
            {
                GameEntity slot = attackerSlots.ElementAt(i);
                
                if (!IsValidAttacker(slot, out GameEntity attackerCard))
                    continue;

                GameEntity target = FindAttackTarget(slot, defender);

                if (target != null)
                {
                    delay += _gameConfig.AnimationTiming.DelayBetweenAttacks;

                    var delay1 = delay;
                    
                    CreateEntity
                        .Request()
                        .AddAttackRequest(attackerCard.Id,target.Id,attackerCard.Damage)
                        .With(x => x.PutOnCooldown(delay1))
                        ;
                }
            }
            
            CreateEntity
                .Request()
                .With(x => x.isSwitchTurnRequest = true)
                .With(x => x.PutOnCooldown(delay + _gameConfig.AnimationTiming.PostAttackDelay))
                ;
        }


        private (GameEntity attacker, GameEntity defender) GetAttackerAndDefender()
        {
            GameEntity hero = null;
            GameEntity enemy = null;

            foreach (var h in _heroes)
            {
                hero = h;
            }

            foreach (var e in _enemies)
            {
                enemy = e;
            }

            return enemy?.isEnemyTurn == true ? (enemy, hero) : (hero, enemy);
        }

        private IEnumerable<GameEntity> GetAttackerSlots(GameEntity attacker)
        {
            return _slots.GetEntities()
                .Where(s => s.hasSlotOwner && s.hasSlotLane && s.SlotOwner == attacker.Id)
                .OrderBy(s => s.SlotLane);
        }

        private bool IsValidAttacker(GameEntity slot, out GameEntity card)
        {
            card = null;

            if (!slot.isOccupied || slot.OccupiedBy < 0)
                return false;

            card = _game.GetEntityWithId(slot.OccupiedBy);
            return card is { isDestructed: false, hasDamage: true };
        }

        private GameEntity FindAttackTarget(GameEntity slot, GameEntity defender)
        {
            var oppositeSlot = BoardHelpers.FindOppositeSlot(_game, slot);

            return TryGetDefenderCard(oppositeSlot, out GameEntity defenderCard) ? defenderCard : defender;
        }

        private bool TryGetDefenderCard(GameEntity oppositeSlot, out GameEntity targetCard)
        {
            if (oppositeSlot is not { isOccupied: true, OccupiedBy: >= 0 })
            {
                targetCard = null;
                return false;
            }

            targetCard = _game.GetEntityWithId(oppositeSlot.OccupiedBy);

            if (targetCard is { isDestructed: false })
            {
                return true;
            }

            targetCard = null;
            return false;
        }
    }
}