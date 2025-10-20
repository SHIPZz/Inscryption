using System.Collections.Generic;
using Code.Common;
using Code.Common.Extensions;
using Code.Common.Services;

namespace Code.Features.Enemy.Services
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;

        public EnemyFactory(GameContext game, IIdService idService)
        {
            _game = game;
            _idService = idService;
        }

        public GameEntity CreateEnemy(int baseHealth)
        {
            return _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isEnemy = true)
                .With(x => x.AddHp(baseHealth))
                .With(x => x.AddMaxHp(baseHealth))
                .With(x => x.AddCardsInHand(new List<int>()));
        }
    }
}

