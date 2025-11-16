using UnityEngine;

namespace Code.Infrastructure.Level
{
    public class LevelProvider : MonoBehaviour, ILevelProvider
    {
        [Header("Hand Parents")]
        [SerializeField] private Transform _heroCardParent;
        [SerializeField] private Transform _enemyCardParent;

        [Header("Deck Stack Parent (Common for both players)")]
        [SerializeField] private Transform _deckStackParent;

        [Header("Slots Parent")]
        [SerializeField] private Transform _slotsParent;
        
        public Transform HeroCardParent => _heroCardParent;
        public Transform EnemyCardParent => _enemyCardParent;
        public Transform DeckStackParent => _deckStackParent;
        public Transform SlotsParent => _slotsParent;
    }
}
