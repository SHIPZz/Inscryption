using UnityEngine;

namespace Code.Infrastructure.Level
{
    public interface ILevelProvider
    {
        Transform HeroCardParent { get; }
        Transform EnemyCardParent { get; }
        Transform DeckStackParent { get; }
        Transform SlotsParent { get; }
    }
}
