using System.Collections.Generic;
using Code.Features.Board;
using Code.Features.Enemy.Services;
using Code.Features.Hero.Services;
using Entitas;

namespace Code.Features.Battle.Systems
{
    public class CreateAttackRequestOnCardPlacedSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;
        private readonly IEnemyProvider _enemyProvider;
        private readonly IGroup<GameEntity> _placedCards;
        private readonly List<GameEntity> _placedCardsBuffer = new(8);

        public CreateAttackRequestOnCardPlacedSystem(GameContext game, IHeroProvider heroProvider, IEnemyProvider enemyProvider) : base(game)
        {
            _game = game;
            _heroProvider = heroProvider;
            _enemyProvider = enemyProvider;
            _placedCards = game.GetGroup(GameMatcher.AllOf(GameMatcher.Card, GameMatcher.Placed));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Placed.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isCard && entity.isPlaced && entity.hasSlotId && entity.hasSlotLane && entity.hasDamage;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var placedCardEntity in entities)
            {
                var placedSlotEntity = _game.GetEntityWithId(placedCardEntity.SlotId);
              
                if (placedSlotEntity == null || !placedSlotEntity.isBoardSlot)
                    continue;

                var oppositeSlotEntity = BoardHelpers.FindOppositeSlot(_game, placedSlotEntity);

                var oppositeSlotCardEntity = FindCardInSlot(oppositeSlotEntity);
                
                if (oppositeSlotCardEntity != null)
                {
                    _game.CreateEntity().AddAttackRequest(placedCardEntity.Id, oppositeSlotCardEntity.Id, placedCardEntity.Damage);
                    continue;
                }

                var faceTargetEntity = GetFaceTarget(oppositeSlotEntity ?? placedSlotEntity);
                
                if (faceTargetEntity != null)
                {
                    _game.CreateEntity().AddAttackRequest(placedCardEntity.Id, faceTargetEntity.Id, placedCardEntity.Damage);
                }
            }
        }

        private GameEntity FindCardInSlot(GameEntity slotEntity)
        {
            if (slotEntity == null)
                return null;

            foreach (var candidateCardEntity in _placedCards.GetEntities(_placedCardsBuffer))
            {
                if (candidateCardEntity.SlotId == slotEntity.Id)
                    return candidateCardEntity;
            }

            return null;
        }

        private GameEntity GetFaceTarget(GameEntity referenceSlotEntity)
        {
            bool isHeroSide = referenceSlotEntity.isHeroOwner;

            if (isHeroSide)
                return _heroProvider.GetHero();

            return _enemyProvider.GetEnemy();

        }
    }
}


