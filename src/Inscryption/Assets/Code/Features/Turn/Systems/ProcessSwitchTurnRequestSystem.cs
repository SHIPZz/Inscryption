using System.Collections.Generic;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.Systems
{
    public class ProcessSwitchTurnRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _switchTurnRequests;
        private readonly List<GameEntity> _buffer = new(4);

        public ProcessSwitchTurnRequestSystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _game = game;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;

            _switchTurnRequests = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.SwitchTurnRequest));
        }

        public void Execute()
        {
            if (_switchTurnRequests.count == 0)
                return;

            bool turnSwitched = false;

            foreach (GameEntity request in _switchTurnRequests.GetEntities(_buffer))
            {
                if (!turnSwitched)
                {
                    GameEntity hero = _heroProvider.GetHero();
                    GameEntity enemy = _enemyProvider.GetEnemy();

                    if (hero == null || enemy == null)
                    {
                        Debug.LogWarning("[ProcessSwitchTurnRequestSystem] Hero or Enemy not found!");
                        
                        request.isDestructed = true;
                        
                        continue;
                    }

                    if (hero.isHeroTurn)
                    {
                        SwitchTurn(hero, enemy, "Enemy");
                        turnSwitched = true;
                    }
                    else if (enemy.isEnemyTurn)
                    {
                        SwitchTurn(enemy, hero, "Hero");
                        turnSwitched = true;
                    }
                }

                request.isDestructed = true;
            }
        }

        private void SwitchTurn(GameEntity currentPlayer, GameEntity nextPlayer, string nextPlayerName)
        {
            currentPlayer.isHeroTurn = false;
            currentPlayer.isEnemyTurn = false;
            
            nextPlayer.isHeroTurn = nextPlayer.isHero;
            nextPlayer.isEnemyTurn = nextPlayer.isEnemy;

            if (currentPlayer.hasCardsPlacedThisTurn)
                currentPlayer.ReplaceCardsPlacedThisTurn(0);
            
            if (nextPlayer.hasCardsPlacedThisTurn)
                nextPlayer.ReplaceCardsPlacedThisTurn(0);

            _game.CreateEntity().AddDrawCardRequest(nextPlayer.Id);

            Debug.Log($"[ProcessSwitchTurnRequestSystem] Turn switched to {nextPlayerName} (ID={nextPlayer.Id})");
        }
    }
}

