using UnityEngine;

namespace Code.Infrastructure.Data
{
	[CreateAssetMenu(fileName = "GameConfig", menuName = "Inscryption/Game Config")]
	public class GameConfig : ScriptableObject
	{
		[Header("Player Settings")]
		[Tooltip("Базовое здоровье героя")]
		[SerializeField] private int _baseHeroHealth = 20;
		
		[Tooltip("Базовое здоровье врага")]
		[SerializeField] private int _baseEnemyHealth = 20;

		[Header("Deck Settings")]
		[Tooltip("Размер колоды (всего карт)")]
		[SerializeField] private int _deckSize = 30;
		
		[Tooltip("Количество карт в стартовой руке")]
		[SerializeField] private int _startingHandSize = 3;

		public float StackVerticalOffset = 0.025f;

		public LayerMask InputMask;

		public int BaseHeroHealth => _baseHeroHealth;
		public int BaseEnemyHealth => _baseEnemyHealth;
		public int DeckSize => _deckSize;
		public int StartingHandSize => _startingHandSize;
	}
}

