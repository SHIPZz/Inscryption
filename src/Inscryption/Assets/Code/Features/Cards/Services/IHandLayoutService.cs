using Code.Features.Layout.Services;
using UnityEngine;

namespace Code.Features.Cards.Services
{
    public interface IHandLayoutService
    {
        CardLayoutData[] CalculateLayout(GameEntity player, int additionalCards = 0);
        Transform GetCardParent(GameEntity player);
        Vector3 GetLastCardPosition(GameEntity player);
    }
}
