using Code.Common;
using Code.Features.Turn.Extensions;
using Code.Infrastructure.Data;
using Code.Infrastructure.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Turn.Systems
{
    public class CreateDrawCardRequestsSystem : IInitializeSystem
    {
        private readonly GameContext _game;
        private readonly GameConfig _gameConfig;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;

        public CreateDrawCardRequestsSystem(
            GameContext game,
            IConfigService configService)
        {
            _game = game;
            _gameConfig = configService.GetConfig<GameConfig>();
            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
        }

        public void Initialize()
        {
            GameEntity player = TurnExtensions.GetCurrentPlayer(_heroes, _enemies);
            if (player == null)
                return;

            int maxHandSize = _gameConfig.GameBalance.MaxHandSize;
            int cardsToDraw = Mathf.Max(0, maxHandSize - player.CardsInHand.Count);

            for (int i = 0; i < cardsToDraw; i++)
            {
                CreateEntity
                    .Request()
                    .AddDrawCardRequest(player.Id);
            }
        }
    }
}

