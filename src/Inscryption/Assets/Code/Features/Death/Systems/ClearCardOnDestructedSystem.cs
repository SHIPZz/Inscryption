using Entitas;

namespace Code.Features.Death.Systems
{
    public class ClearCardOnDestructedSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _destroyedCards;

        public ClearCardOnDestructedSystem(GameContext game)
        {
            _game = game;
            _destroyedCards = game.GetGroup(GameMatcher.AllOf(GameMatcher.Card, GameMatcher.Destructed));
        }

        public void Execute()
        {
            foreach (GameEntity card in _destroyedCards)
            {
                if (card.hasSlotId) 
                    ClearSlot(card);

                if (card.hasCardOwner) 
                    ClearCardOwner(card);
            }
        }

        private void ClearCardOwner(GameEntity card)
        {
            GameEntity owner = _game.GetEntityWithId(card.CardOwner);

            owner?.PlacedCards.Remove(card.Id);
        }

        private void ClearSlot(GameEntity card)
        {
            GameEntity slot = _game.GetEntityWithId(card.SlotId);

            if (slot != null && slot.isBoardSlot && slot.OccupiedBy == card.Id)
            {
                slot.isOccupied = false;
                slot.ReplaceOccupiedBy(-1);
            }

            card.RemoveSlotId();
        }
    }
}
