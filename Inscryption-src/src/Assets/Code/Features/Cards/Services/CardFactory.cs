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

        public GameEntity CreateCard(CardCreateData cardCreateData)
        {
            return _game.CreateEntity()
                .AddId(_idService.Next())
                .With(x => x.isCard = true)
                .With(x => x.AddHp(cardCreateData.Hp))
                .With(x => x.AddMaxHp(cardCreateData.Hp))
                .With(x => x.AddDamage(cardCreateData.Damage))
                .With(x => x.AddCardOwner(cardCreateData.OwnerId))
                .With(x => x.isInHand = cardCreateData.InHand);
        }

        public GameEntity CreateRandomCard(CardCreateData cardCreateData)
        {
            int hp = _randomService.Range(1, 5);
            int damage = _randomService.Range(1, 4);
            return CreateCard(new CardCreateData(cardCreateData.OwnerId, hp, damage,true));
        }
    }

    public struct CardCreateData
    {
        public readonly int OwnerId;
        public readonly int Hp;
        public readonly int Damage;
        public readonly bool InHand;

        public CardCreateData(int ownerId, int hp, int damage, bool inHand = false)
        {
            OwnerId = ownerId;
            Hp = hp;
            Damage = damage;
            InHand = inHand;
        }
    }
}

