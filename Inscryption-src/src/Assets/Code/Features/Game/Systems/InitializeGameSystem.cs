using System.Collections.Generic;
using Code.Features.Board.Services;
using Code.Features.Cards.Services;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
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
        private readonly IBoardFactory _boardFactory;

        private const int BaseHealth = 20;
        private const int StartingHandSize = 3;
        private const int DeckSize = 30;

        public InitializeGameSystem(
            GameContext game,
            IHeroFactory heroFactory,
            IEnemyFactory enemyFactory,
            ICardFactory cardFactory,
            IBoardFactory boardFactory)
        {
            _game = game;
            _heroFactory = heroFactory;
            _enemyFactory = enemyFactory;
            _cardFactory = cardFactory;
            _boardFactory = boardFactory;
        }

        public void Initialize()
        {
            Debug.Log("[InitializeGameSystem] Starting game initialization...");

            GameEntity hero = _heroFactory.CreateHero(BaseHealth);
            Debug.Log($"[InitializeGameSystem] Hero created: ID={hero.Id}, HP={hero.Hp}");

            GameEntity enemy = _enemyFactory.CreateEnemy(BaseHealth);
            Debug.Log($"[InitializeGameSystem] Enemy created: ID={enemy.Id}, HP={enemy.Hp}");

            List<GameEntity> slots = _boardFactory.CreateSlots(hero.Id, enemy.Id);
            Debug.Log($"[InitializeGameSystem] Board created: {slots.Count} slots");

            List<GameEntity> deck = CreateDeck(hero.Id, enemy.Id);
            Debug.Log($"[InitializeGameSystem] Deck created: {deck.Count} cards");

            DealStartingHand(hero, deck, StartingHandSize);
            DealStartingHand(enemy, deck, StartingHandSize);

            Debug.Log("[InitializeGameSystem] Game initialization complete. Hero's turn!");
        }

        private List<GameEntity> CreateDeck(int heroId, int enemyId)
        {
            List<GameEntity> deck = new List<GameEntity>();

            for (int i = 0; i < DeckSize / 2; i++)
            {
                deck.Add(_cardFactory.CreateRandomCard(heroId));
                deck.Add(_cardFactory.CreateRandomCard(enemyId));
            }

            return deck;
        }

        private void DealStartingHand(GameEntity player, List<GameEntity> deck, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (deck.Count == 0) break;

                GameEntity card = FindCardInDeck(deck, player.Id);
                if (card != null)
                {
                    player.CardsInHand.Add(card.Id);
                    deck.Remove(card);
                }
            }

            Debug.Log($"[InitializeGameSystem] Player {player.Id} received {player.CardsInHand.Count} cards");
        }

        private GameEntity FindCardInDeck(List<GameEntity> deck, int ownerId)
        {
            foreach (GameEntity card in deck)
            {
                if (card.CardOwner == ownerId)
                    return card;
            }
            return null;
        }
    }
}

