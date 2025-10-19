using Code.Common.Extensions;
using Code.Common.Random;
using Code.Common.Services;

namespace Code.Features.Cards.Services
{
    public class CardFactory : ICardFactory
    {
        private readonly GameContext _game;
        private readonly IIdService _idService;
        private readonly IRandomService _randomService;

        public CardFactory(GameContext game, IIdService idService, IRandomService randomService)
        {
            _game = game;
            _idService = idService;
            _randomService = randomService;
        }

        public GameEntity CreateCard(int hp, int damage, int ownerId)
        {
            return _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isCard = true)
                .With(x => x.AddHp(hp))
                .With(x => x.AddMaxHp(hp))
                .With(x => x.AddDamage(damage))
                .With(x => x.AddCardOwner(ownerId))
                .With(x => x.isInHand = true);
        }

        public GameEntity CreateRandomCard(int ownerId)
        {
            int hp = _randomService.Range(1, 5);
            int damage = _randomService.Range(1, 4);
            return CreateCard(hp, damage, ownerId);
        }
    }
}

