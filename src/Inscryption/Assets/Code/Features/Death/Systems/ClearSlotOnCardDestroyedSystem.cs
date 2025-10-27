using Entitas;

namespace Code.Features.Death.Systems
{
    public class ClearSlotOnCardDestroyedSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _destroyedCards;

        public ClearSlotOnCardDestroyedSystem(GameContext game)
        {
            _game = game;
            _destroyedCards = game.GetGroup(GameMatcher.AllOf(GameMatcher.Card, GameMatcher.Destructed));
        }

        public void Execute()
        {
            foreach (GameEntity card in _destroyedCards)
            {
                if (card.hasSlotId)
                {
                    GameEntity slot = _game.GetEntityWithId(card.SlotId);

                    if (slot != null && slot.isBoardSlot && slot.OccupiedBy == card.Id)
                    {
                        slot.isOccupied = false;
                        slot.ReplaceOccupiedBy(-1);
                    }

                    card.RemoveSlotId();
                }

                if (card.hasCardOwner)
                {
                    GameEntity owner = _game.GetEntityWithId(card.CardOwner);

                    if (owner != null)
                    {
                        owner.PlacedCards.Remove(card.Id);
                    }
                }
            }
        }
    }
}
