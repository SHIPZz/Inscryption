using System;
using Code.Common.Time;
using Code.Features;
using Code.Infrastructure.Systems;
using Entitas;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure
{
    public class GameTestRunner : ITickable, IDisposable
    {
        private readonly ISystemFactory _systemFactory;
        private readonly GameContext _game;
        private readonly ITimeService _timeService;

        private float _turnTimer = 0f;
        private const float TurnDelay = 2f;
        private bool _isInitialized = false;
        private GameRootFeature _gameRootFeature;

        public GameTestRunner(ISystemFactory systemFactory, GameContext game, ITimeService timeService)
        {
            _systemFactory = systemFactory;
            _game = game;
            _timeService = timeService;
        }

        public void Initialize()
        {
            Debug.Log("=== Game Test Runner Started ===");

            _gameRootFeature = _systemFactory.Create<GameRootFeature>();
            _gameRootFeature.Initialize();

            _isInitialized = true;

            Debug.Log("=== Game Initialized ===");
        }

        public void Tick()
        {
            if (!_isInitialized)
                return;

            _gameRootFeature?.Execute();
            _gameRootFeature?.Cleanup();

            _turnTimer += _timeService.DeltaTime;

            if (_turnTimer >= TurnDelay)
            {
                _turnTimer = 0f;

                GameEntity enemy = GetEnemy();
                if (enemy != null && enemy.isEnemyTurn)
                {
                    Debug.Log($"[GameTestRunner] Auto-ending enemy turn after {TurnDelay}s");
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameEntity hero = GetHero();
                if (hero != null && hero.isHeroTurn)
                {
                    Debug.Log("[GameTestRunner] Player pressed SPACE - ending turn");
                    _game.CreateEntity().isEndTurnRequest = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TryPlaceHeroCard(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TryPlaceHeroCard(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TryPlaceHeroCard(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TryPlaceHeroCard(3);
            }
        }

        private void TryPlaceHeroCard(int laneIndex)
        {
            GameEntity hero = GetHero();
            if (hero == null || !hero.isHeroTurn)
            {
                Debug.Log("[GameTestRunner] Not hero's turn");
                return;
            }

            if (!hero.hasCardsInHand || hero.CardsInHand.Count == 0)
            {
                Debug.Log("[GameTestRunner] Hero has no cards in hand");
                return;
            }

            int cardId = hero.CardsInHand[0];
            GameEntity slot = GetHeroSlot(hero.Id, laneIndex);

            if (slot == null)
            {
                Debug.Log($"[GameTestRunner] Slot not found for lane {laneIndex}");
                return;
            }

            _game.CreateEntity().AddPlaceCardRequest(cardId, slot.Id);
            Debug.Log($"[GameTestRunner] Placing card {cardId} on lane {laneIndex}");
        }

        private GameEntity GetHero()
        {
            foreach (GameEntity entity in _game.GetEntities(GameMatcher.Hero))
            {
                if (!entity.isDestructed)
                    return entity;
            }
            return null;
        }

        private GameEntity GetEnemy()
        {
            foreach (GameEntity entity in _game.GetEntities(GameMatcher.Enemy))
            {
                if (!entity.isDestructed)
                    return entity;
            }
            return null;
        }

        private GameEntity GetHeroSlot(int heroId, int laneIndex)
        {
            foreach (GameEntity slot in _game.GetEntities(GameMatcher.BoardSlot))
            {
                if (slot.SlotOwner == heroId && slot.SlotLane == laneIndex)
                    return slot;
            }
            return null;
        }

        public void Dispose()
        {
            _gameRootFeature?.DeactivateReactiveSystems();
            _gameRootFeature?.TearDown();
            _gameRootFeature = null;

            Debug.Log("=== Game Test Runner Destroyed ===");
        }
    }
}

