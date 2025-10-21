using System.Collections.Generic;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Battle.Systems
{
    public class ProcessAttackPhaseSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _attackPhases;
        private readonly IGroup<GameEntity> _switchTurnRequests;
        private readonly IGroup<GameEntity> _cardsOnBoard;
        private readonly List<GameEntity> _buffer = new(8);

        public ProcessAttackPhaseSystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _game = game;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;

            _attackPhases = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.AttackPhase));

            _switchTurnRequests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.SwitchTurnRequest));

            _cardsOnBoard = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Card, GameMatcher.OnBoard, GameMatcher.Lane)
                .NoneOf(GameMatcher.Destructed));
        }

        public void Execute()
        {
            if (_attackPhases.count == 0)
                return;

            if (_switchTurnRequests.count > 0)
            {
                foreach (GameEntity phase in _attackPhases.GetEntities(_buffer))
                {
                    phase.isDestructed = true;
                }
                return;
            }

            GameEntity firstPhase = _attackPhases.GetEntities(_buffer)[0];

            GameEntity activePlayer = GetActivePlayer();

            if (activePlayer == null)
            {
                Debug.LogWarning("[ProcessAttackPhaseSystem] No active player found!");
                firstPhase.isDestructed = true;
                return;
            }

            Debug.Log($"[ProcessAttackPhaseSystem] Processing attacks for player {activePlayer.Id}");

            List<GameEntity> attackingCards = GetPlayerCardsOnBoard(activePlayer.Id);

            foreach (GameEntity attacker in attackingCards)
            {
                GameEntity target = FindTargetForAttacker(attacker, activePlayer.Id);

                if (target != null)
                {
                    CreateAttackRequest(attacker, target);
                }
                else
                {
                    GameEntity opponent = GetOpponent(activePlayer);
                    if (opponent != null)
                    {
                        CreateAttackRequest(attacker, opponent);
                    }
                }
            }

            _game.CreateEntity().isSwitchTurnRequest = true;

            foreach (GameEntity phase in _attackPhases.GetEntities(_buffer))
            {
                phase.isDestructed = true;
            }
        }

        private GameEntity GetActivePlayer()
        {
            GameEntity hero = _heroProvider.GetActiveHero();
            if (hero != null)
                return hero;

            GameEntity enemy = _enemyProvider.GetActiveEnemy();
            if (enemy != null)
                return enemy;

            return null;
        }

        private GameEntity GetOpponent(GameEntity player)
        {
            if (player.isHero)
                return _enemyProvider.GetEnemy();
            
            if (player.isEnemy)
                return _heroProvider.GetHero();

            return null;
        }

        private List<GameEntity> GetPlayerCardsOnBoard(int playerId)
        {
            List<GameEntity> cards = new List<GameEntity>();

            foreach (GameEntity card in _cardsOnBoard)
            {
                if (card.CardOwner == playerId)
                    cards.Add(card);
            }

            cards.Sort((a, b) => a.Lane.CompareTo(b.Lane));

            return cards;
        }

        private GameEntity FindTargetForAttacker(GameEntity attacker, int attackerOwnerId)
        {
            int attackerLane = attacker.Lane;

            foreach (GameEntity card in _cardsOnBoard)
            {
                if (card.CardOwner != attackerOwnerId && card.Lane == attackerLane)
                {
                    return card;
                }
            }

            return null;
        }

        private void CreateAttackRequest(GameEntity attacker, GameEntity target)
        {
            _game.CreateEntity()
                .AddAttackRequest(attacker.Id, target.Id, attacker.Damage);

            Debug.Log($"[ProcessAttackPhaseSystem] Created attack: Card {attacker.Id} -> Target {target.Id} (Damage: {attacker.Damage})");
        }
    }
}

