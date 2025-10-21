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
        private readonly IGroup<GameEntity> _endTurnRequests;
        private readonly IGroup<GameEntity> _placeCardRequests;
        private readonly IGroup<GameEntity> _boardSlots;

        public EnemyAISystem(GameContext game, IRandomService randomService)
        {
            _game = game;
            _randomService = randomService;

            _enemies = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Enemy, GameMatcher.EnemyTurn)
                .NoneOf(GameMatcher.Destructed));

            _endTurnRequests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.EndTurnRequest));

            _placeCardRequests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.PlaceCardRequest));

            _boardSlots = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.BoardSlot));
        }

        public void Execute()
        {
            if (_enemies.count == 0)
                return;

            if (_endTurnRequests.count > 0)
                return;

            if (_placeCardRequests.count > 0)
                return;

            foreach (GameEntity enemy in _enemies)
            {
                if (!enemy.hasCardsInHand || enemy.CardsInHand.Count == 0)
                {
                    Debug.Log($"[EnemyAISystem] Enemy {enemy.Id} has no cards, ending turn");
                    _game.CreateEntity().isEndTurnRequest = true;
                    return;
                }

                if (enemy.hasCardsPlacedThisTurn && enemy.CardsPlacedThisTurn >= 1)
                {
                    Debug.Log($"[EnemyAISystem] Enemy {enemy.Id} already placed card, ending turn");
                    _game.CreateEntity().isEndTurnRequest = true;
                    return;
                }

                List<int> cardsInHand = enemy.CardsInHand;
                List<GameEntity> availableSlots = ListPool<GameEntity>.Get();

                GetAvailableSlots(enemy.Id, availableSlots);

                if (availableSlots.Count == 0)
                {
                    Debug.Log($"[EnemyAISystem] Enemy {enemy.Id} has no available slots, ending turn");
                    ListPool<GameEntity>.Release(availableSlots);
                    _game.CreateEntity().isEndTurnRequest = true;
                    return;
                }

                int randomCardId = cardsInHand[_randomService.Range(0, cardsInHand.Count)];
                GameEntity randomSlot = availableSlots[_randomService.Range(0, availableSlots.Count)];

                _game.CreateEntity().AddPlaceCardRequest(randomCardId, randomSlot.Id);

                Debug.Log($"[EnemyAISystem] Enemy {enemy.Id} placing card {randomCardId} on slot {randomSlot.Id}");

                ListPool<GameEntity>.Release(availableSlots);
            }
        }

        private void GetAvailableSlots(int ownerId, List<GameEntity> resultSlots)
        {
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

