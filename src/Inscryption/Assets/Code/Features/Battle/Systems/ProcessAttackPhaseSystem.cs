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
        private readonly List<GameEntity> _playerCardsCache = new();
        private readonly Dictionary<int, GameEntity> _enemyCardsByLane = new();

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

            BuildAttackCaches(activePlayer.Id);

            foreach (GameEntity attacker in _playerCardsCache)
            {
                GameEntity target = FindTargetForAttacker(attacker);

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

        private void BuildAttackCaches(int playerId)
        {
            _playerCardsCache.Clear();
            _enemyCardsByLane.Clear();

            foreach (GameEntity card in _cardsOnBoard)
            {
                if (card.CardOwner == playerId)
                {
                    _playerCardsCache.Add(card);
                }
                else
                {
                    _enemyCardsByLane[card.Lane] = card;
                }
            }

            _playerCardsCache.Sort((a, b) => a.Lane.CompareTo(b.Lane));
        }

        private GameEntity FindTargetForAttacker(GameEntity attacker)
        {
            _enemyCardsByLane.TryGetValue(attacker.Lane, out GameEntity target);
            return target;
        }

        private void CreateAttackRequest(GameEntity attacker, GameEntity target)
        {
            _game.CreateEntity()
                .AddAttackRequest(attacker.Id, target.Id, attacker.Damage);

            Debug.Log($"[ProcessAttackPhaseSystem] Created attack: Card {attacker.Id} -> Target {target.Id} (Damage: {attacker.Damage})");
        }
    }
}

