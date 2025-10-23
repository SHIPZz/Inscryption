using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Common.Random;
using Code.Features.Board.Services;
using Code.Features.Cards.Data;
using Code.Features.Cards.Services;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Code.Features.Layout.Services;
using Code.Infrastructure.Data;
using Code.Infrastructure.Level;
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
        private readonly ICardFactory _cardFactory;
        private readonly ICardStackFactory _cardStackFactory;
        private readonly IBoardFactory _boardFactory;
        private readonly IConfigService _configService;
        private readonly IRandomService _randomService;
        private readonly ILevelProvider _levelProvider;

        private GameConfig _gameConfig;

        private IReadOnlyList<Vector3> _heroHandPositions;
        private IReadOnlyList<Vector3> _enemyHandPositions;

        public InitializeGameSystem(
            GameContext game,
            IHeroFactory heroFactory,
            IEnemyFactory enemyFactory,
            ICardFactory cardFactory,
            ICardStackFactory cardStackFactory,
            IBoardFactory boardFactory,
            IConfigService configService,
            IRandomService randomService,
            ILevelProvider levelProvider)
        {
            _game = game;
            _heroFactory = heroFactory;
            _enemyFactory = enemyFactory;
            _cardFactory = cardFactory;
            _cardStackFactory = cardStackFactory;
            _boardFactory = boardFactory;
            _configService = configService;
            _randomService = randomService;
            _levelProvider = levelProvider;
        }

        public void Initialize()
        {
            Debug.Log("[InitializeGameSystem] Starting game initialization...");

            _gameConfig = _configService.GetConfig<GameConfig>(nameof(GameConfig));
            
            if (_gameConfig == null)
            {
                Debug.LogError("[InitializeGameSystem] GameConfig not found! Using default values.");
                return;
            }

            CalculateHandPositions();

            GameEntity hero = _heroFactory.CreateHero(_gameConfig.BaseHeroHealth);
            Debug.Log($"[InitializeGameSystem] Hero created: ID={hero.Id}, HP={hero.Hp}");

            GameEntity enemy = _enemyFactory.CreateEnemy(_gameConfig.BaseEnemyHealth);
            Debug.Log($"[InitializeGameSystem] Enemy created: ID={enemy.Id}, HP={enemy.Hp}");

            List<GameEntity> slots = _boardFactory.CreateSlots(hero.Id, enemy.Id);
            Debug.Log($"[InitializeGameSystem] Board created: {slots.Count} slots");

            List<DeckCard> deck = CreateDeck(hero.Id, enemy.Id);
            Debug.Log($"[InitializeGameSystem] Deck created: {deck.Count} cards");

            GameEntity commonStack = CreateCardStack(-1);
            Debug.Log($"[InitializeGameSystem] Common card stack created: ID={commonStack.Id}");

            DealStartingHand(hero, deck, _gameConfig.StartingHandSize, true);
            DealStartingHand(enemy, deck, _gameConfig.StartingHandSize);

            Debug.Log("[InitializeGameSystem] Game initialization complete. Hero's turn!");
        }
        
        private void CalculateHandPositions()
        {
            var layoutParams = new HorizontalLayoutParams
            {
                Count = _gameConfig.StartingHandSize,
                Spacing = 1.5f,
                Origin = new Vector3(0, 0, -5f)
            };
            _heroHandPositions = PositionCalculator.CalculateHorizontalLayoutPositions(layoutParams);

            layoutParams.Origin = new Vector3(0, 0, 5f);
            _enemyHandPositions = PositionCalculator.CalculateHorizontalLayoutPositions(layoutParams);
        }

        private List<DeckCard> CreateDeck(int heroId, int enemyId)
        {
            List<DeckCard> deck = new List<DeckCard>();
            CardConfig cardConfig = _configService.GetConfig<CardConfig>(nameof(CardConfig));

            if (cardConfig == null || cardConfig.Cards == null || cardConfig.Cards.Count == 0)
            {
                Debug.LogError("[InitializeGameSystem] CardConfig or Cards list is empty!");
                return deck;
            }

            for (int i = 0; i < _gameConfig.DeckSize / 2; i++)
            {
                CardData heroCardData = cardConfig.Cards[_randomService.Range(0, cardConfig.Cards.Count)];
                CardData enemyCardData = cardConfig.Cards[_randomService.Range(0, cardConfig.Cards.Count)];

                deck.Add(new DeckCard(heroId, heroCardData));
                deck.Add(new DeckCard(enemyId, enemyCardData));
            }

            return deck;
        }

        private List<GameEntity> DealStartingHand(GameEntity player, List<DeckCard> deck, int count, bool heroOwner = false)
        {
            int cardsDealt = 0;
            List<GameEntity> created = new List<GameEntity>(count);

            for (int i = 0; i < count; i++)
            {
                if (deck.Count == 0)
                    break;

                int cardIndexInDeck = FindCardIndexInDeck(deck, player.Id);

                if (cardIndexInDeck == -1)
                    break;

                DeckCard deckCard = deck[cardIndexInDeck];
                deck.RemoveAt(cardIndexInDeck);

                GameEntity card = CreateCardInHand(player, deckCard, cardsDealt, heroOwner);
                player.CardsInHand.Add(card.Id);
                created.Add(card);

                cardsDealt++;
            }

            Debug.Log($"[InitializeGameSystem] Player {player.Id} received {cardsDealt} cards");
            return created;
        }

        private GameEntity CreateCardInHand(GameEntity player, DeckCard deckCard, int handIndex, bool heroOwner)
        {
            Vector3 localHandPosition = GetHandPosition(player.isHero, handIndex);
            Transform parent = heroOwner ? _levelProvider.HeroCardParent : _levelProvider.EnemyCardParent;

            GameEntity card = _cardFactory.CreateCard(new CardCreateData(
                    deckCard.OwnerId,
                    deckCard.CardData.Hp,
                    deckCard.CardData.Damage,
                    inHand: true,
                    icon: deckCard.CardData.VisualData?.Icon,
                    position: Vector3.zero, // Position will be handled by LocalPosition system
                    isHeroOwner: heroOwner,
                    parent: parent));

            card.AddLocalPosition(localHandPosition);

            return card.With(x => x.isHeroOwner = heroOwner)
                .With(x => x.isEnemyOwner = !heroOwner);
        }

        private Vector3 GetHandPosition(bool isHero, int cardIndex)
        {
            return isHero
                ? _heroHandPositions[cardIndex]
                : _enemyHandPositions[cardIndex];
        }

        private int FindCardIndexInDeck(List<DeckCard> deck, int ownerId)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                if (deck[i].OwnerId == ownerId)
                    return i;
            }

            return -1;
        }

        private GameEntity CreateCardStack(int ownerId)
        {
            Vector3 stackPosition = new Vector3(7, 0, 0);

            return _cardStackFactory.CreateCardStack(new CardStackCreateData(
                cardCount: _gameConfig.DeckSize,
                ownerId: ownerId,
                position: stackPosition,
                verticalOffset: 0.05f,
                isHero: false
            ));
        }

        private struct DeckCard
        {
            public readonly int OwnerId;
            public readonly CardData CardData;

            public DeckCard(int ownerId = -1, CardData cardData = null)
            {
                OwnerId = ownerId;
                CardData = cardData;
            }
        }
    }
}

