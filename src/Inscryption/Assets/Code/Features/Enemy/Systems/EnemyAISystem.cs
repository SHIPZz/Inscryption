using System.Collections.Generic;
using Code.Common.Random;
using Entitas;
using UnityEngine;
using UnityEngine.Pool;

namespace Code.Features.Enemy.Systems
{
    public class EnemyAISystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IRandomService _randomService;
        private readonly IGroup<GameEntity> _enemies;
        private readonly IGroup<GameEntity> _boardSlots;

        public EnemyAISystem(GameContext game, IRandomService randomService)
        {
            _game = game;
            _randomService = randomService;

            _enemies = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Enemy, GameMatcher.EnemyTurn)
                .NoneOf(GameMatcher.Destructed));
            
            _boardSlots = game.GetGroup(GameMatcher.AllOf(GameMatcher.BoardSlot));
        }

        public void Execute()
        {
            foreach (GameEntity enemy in _enemies.GetEntities())
            {
                if (enemy.hasCardsPlacedThisTurn && enemy.CardsPlacedThisTurn >= 1)
                {
                    _game.CreateEntity().isEndTurnRequest = true;
                    continue;
                }
                
                if (!enemy.hasCardsInHand || enemy.CardsInHand.Count == 0)
                {
                    _game.CreateEntity().isEndTurnRequest = true;
                    continue;
                }

                List<GameEntity> availableSlots = ListPool<GameEntity>.Get();
                GetAvailableSlots(enemy.Id, availableSlots);

                if (availableSlots.Count > 0)
                {
                    int randomCardId = enemy.CardsInHand[_randomService.Range(0, enemy.CardsInHand.Count)];
                    GameEntity randomSlot = availableSlots[_randomService.Range(0, availableSlots.Count)];
                    _game.CreateEntity().AddPlaceCardRequest(randomCardId, randomSlot.Id);
                    Debug.Log($"[EnemyAISystem] Enemy {enemy.Id} placing card {randomCardId} on slot {randomSlot.Id}");
                    _game.CreateEntity().isEndTurnRequest = true;
                }
                else
                {
                    _game.CreateEntity().isEndTurnRequest = true;
                }

                ListPool<GameEntity>.Release(availableSlots);
            }
        }

        private void GetAvailableSlots(int ownerId, List<GameEntity> resultSlots)
        {
            resultSlots.Clear();
            foreach (GameEntity slot in _boardSlots)
            {
                if (slot.SlotOwner == ownerId && slot.OccupiedBy == -1)
                {
                    resultSlots.Add(slot);
                }
            }
        }
    }
}

