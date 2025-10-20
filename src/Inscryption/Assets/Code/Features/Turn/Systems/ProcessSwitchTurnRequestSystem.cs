using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.Systems
{
    public class ProcessSwitchTurnRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _switchTurnRequests;
        private readonly List<GameEntity> _buffer = new(4);

        public ProcessSwitchTurnRequestSystem(GameContext game)
        {
            _game = game;

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
                    GameEntity hero = GetHero();
                    GameEntity enemy = GetEnemy();

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
            if (currentPlayer.isHero)
                currentPlayer.isHeroTurn = false;
            else if (currentPlayer.isEnemyTurn)
                currentPlayer.isEnemyTurn = false;

            if (nextPlayer.isHero)
                nextPlayer.isHeroTurn = true;
            else if (nextPlayer.isEnemy)
                nextPlayer.isEnemyTurn = true;

            if (currentPlayer.hasCardsPlacedThisTurn)
                currentPlayer.ReplaceCardsPlacedThisTurn(0);
            
            if (nextPlayer.hasCardsPlacedThisTurn)
                nextPlayer.ReplaceCardsPlacedThisTurn(0);

            _game.CreateEntity().AddDrawCardRequest(nextPlayer.Id);

            Debug.Log($"[ProcessSwitchTurnRequestSystem] Turn switched to {nextPlayerName} (ID={nextPlayer.Id})");
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
    }
}

