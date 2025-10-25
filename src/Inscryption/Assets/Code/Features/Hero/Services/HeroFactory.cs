using System.Collections.Generic;
using Code.Common;
using Code.Common.Extensions;
using Code.Common.Services;
using Code.Features.Stats;

namespace Code.Features.Hero.Services
{
    public class HeroFactory : IHeroFactory
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;

        public HeroFactory(GameContext game, IIdService _idService)
        {
            _game = game;
            this._idService = _idService;
        }

        public GameEntity CreateHero(int baseHealth)
        {
            return _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isHero = true)
                .With(x => x.AddHp(baseHealth))
                .With(x => x.AddMaxHp(baseHealth))
                .With(x => x.AddStats(new Dictionary<StatTypeId, int> { { StatTypeId.Hp, baseHealth } }))
                .With(x => x.AddStatsModifiers(new Dictionary<StatTypeId, int>()))
                .With(x => x.isHeroTurn = true)
                .With(x => x.AddCardsInHand(new List<int>()))
                .With(x => x.AddCardsPlacedThisTurn(0));
        }
    }
}

