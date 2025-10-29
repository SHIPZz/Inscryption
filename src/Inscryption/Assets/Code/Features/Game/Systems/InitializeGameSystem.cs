using System.Collections.Generic;
using Code.Common;
using Code.Features.Board.Services;
using Code.Features.Cards.Services;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Game.Systems
{
    public class InitializeGameSystem : IInitializeSystem
    {
        private readonly GameContext _game;
        private readonly IHeroFactory _heroFactory;
        private readonly IEnemyFactory _enemyFactory;
        private readonly ICardStackFactory _cardStackFactory;
        private readonly IBoardFactory _boardFactory;
        private readonly IHandLayoutService _handLayoutService;
        private readonly GameConfig _gameConfig;

        public InitializeGameSystem(
            GameContext game,
            IHeroFactory heroFactory,
            IEnemyFactory enemyFactory,
            ICardStackFactory cardStackFactory,
            IBoardFactory boardFactory,
            IHandLayoutService handLayoutService,
            IConfigService configService)
        {
            _game = game;
            _heroFactory = heroFactory;
            _enemyFactory = enemyFactory;
            _cardStackFactory = cardStackFactory;
            _boardFactory = boardFactory;
            _handLayoutService = handLayoutService;
            _gameConfig = configService.GetConfig<GameConfig>();
        }

        public void Initialize()
        {
            Debug.Log("[InitializeGameSystem] Starting game initialization...");
            if (_gameConfig == null)
            {
                Debug.LogError("[InitializeGameSystem] GameConfig not found! Using default values.");
                return;
            }

            GameEntity hero = _heroFactory.CreateHero(_gameConfig.BaseHeroHealth);
            Debug.Log($"[InitializeGameSystem] Hero created: ID={hero.Id}, HP={hero.Hp}");
            GameEntity enemy = _enemyFactory.CreateEnemy(_gameConfig.BaseEnemyHealth);
            Debug.Log($"[InitializeGameSystem] Enemy created: ID={enemy.Id}, HP={enemy.Hp}");
            List<GameEntity> slots = _boardFactory.CreateSlots(hero.Id, enemy.Id);
            Debug.Log($"[InitializeGameSystem] Board created: {slots.Count} slots");
            GameEntity commonStack = CreateCardStack();
            Debug.Log($"[InitializeGameSystem] Common card stack created: ID={commonStack.Id}");

            CreateDrawCardFromStackRequest(commonStack, hero);
            CreateDrawCardFromStackRequest(commonStack, enemy);

            Debug.Log("[InitializeGameSystem] Game initialization complete. Hero's turn!");
        }

        private GameEntity CreateDrawCardFromStackRequest(GameEntity commonStack, GameEntity player)
        {
            var animTiming = _gameConfig.AnimationTiming;
            var parentTransform = _handLayoutService.GetCardParent(player);
            var targetPosition = parentTransform.position;

            return CreateEntity.Request()
                .AddDrawCardFromStackRequest(
                    commonStack.Id,
                    newCardsToDraw: _gameConfig.StartingHandSize,
                    player.Id,
                    targetPosition,
                    animTiming.DelayBetweenCards,
                    animTiming.CardMoveDuration,
                    parentTransform);
        }

        private GameEntity CreateCardStack()
        {
            return _cardStackFactory.CreateCardStack(new CardStackFactory.CardStackCreateData(
                cardCount: _gameConfig.DeckSize,
                position: Vector3.zero,
                verticalOffset: _gameConfig.StackVerticalOffset
            ));
        }
    }
}