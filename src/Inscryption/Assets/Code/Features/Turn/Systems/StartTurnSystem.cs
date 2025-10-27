using System.Collections.Generic;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;

namespace Code.Features.Turn.Systems
{
    public class StartTurnSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _switchTurnRequests;
        private readonly List<GameEntity> _requestBuffer = new(4);

        private const int MaxHandSize = 5;

        public StartTurnSystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider)
        {
            _game = game;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
            _switchTurnRequests = game.GetGroup(GameMatcher.SwitchTurnRequest);
        }

        public void Execute()
        {
            foreach (GameEntity request in _switchTurnRequests.GetEntities(_requestBuffer))
            {
                HandleTurnStart();
                request.isDestructed = true;
            }
        }

        private void HandleTurnStart()
        {
            GameEntity hero = _heroProvider.GetHero();
            GameEntity enemy = _enemyProvider.GetEnemy();

            if (hero != null && hero.isHeroTurn)
            {
                ProcessHeroTurn(hero);
                return;
            }

            if (enemy != null && enemy.isEnemyTurn)
            {
                ProcessEnemyTurn(enemy);
            }
        }

        private void ProcessHeroTurn(GameEntity hero)
        {
            if (hero.hasCardsPlacedThisTurn)
                hero.ReplaceCardsPlacedThisTurn(0);

            if (hero.CardsInHand.Count < MaxHandSize)
                _game.CreateEntity().AddDrawCardRequest(hero.Id);
        }

        private void ProcessEnemyTurn(GameEntity enemy)
        {
            if (enemy.hasCardsPlacedThisTurn)
                enemy.ReplaceCardsPlacedThisTurn(0);

            if (enemy.CardsInHand.Count < MaxHandSize)
                _game.CreateEntity().AddDrawCardRequest(enemy.Id);
        }
    }
}
