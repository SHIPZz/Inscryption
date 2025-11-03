using System.Collections.Generic;
using System.Linq;
using Entitas;

namespace Code.Features.Board.Extensions
{
  public static class BoardSlotExtensions
  {
    public static IEnumerable<GameEntity> GetOwnedSlots(this IGroup<GameEntity> slots, int ownerId)
    {
      return slots.GetEntities()
        .Where(s => s.hasSlotOwner && s.hasSlotLane && s.SlotOwner == ownerId)
        .OrderBy(s => s.SlotLane);
    }

    public static bool TryGetOccupyingCard(this GameEntity slot, out GameEntity card)
    {
      card = null;

      if (!slot.isOccupied || slot.OccupiedBy < 0)
        return false;

      card = Contexts.sharedInstance.game.GetEntityWithId(slot.OccupiedBy);
      return card is { isDestructed: false, hasDamage: true };
    }

    public static GameEntity FindOppositeTarget(this GameEntity slot, GameEntity fallbackTarget)
    {
      GameContext game = Contexts.sharedInstance.game;
      GameEntity oppositeSlot = BoardHelpers.FindOppositeSlot(game, slot);

      if (oppositeSlot.TryGetDefendingCard(out GameEntity defenderCard))
        return defenderCard;

      return fallbackTarget;
    }

    public static bool TryGetDefendingCard(this GameEntity slot, out GameEntity card)
    {
      card = null;

      if (slot is not { isOccupied: true, OccupiedBy: >= 0 })
        return false;

      card = Contexts.sharedInstance.game.GetEntityWithId(slot.OccupiedBy);
      return card is { isDestructed: false };
    }
  }
}
