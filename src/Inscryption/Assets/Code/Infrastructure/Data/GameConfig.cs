using System;
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

		[Header("Layout Settings")]
		[SerializeField] private HandLayoutData _handLayout = new();
		[SerializeField] private BoardLayoutData _boardLayout = new();

		[Header("Animation Settings")]
		[SerializeField] private AnimationTimingData _animationTiming = new();

		[Header("Game Balance")]
		[SerializeField] private GameBalanceData _gameBalance = new();

		[Header("Card Generation")]
		[SerializeField] private CardGenerationData _cardGeneration = new();

		[Header("Enemy AI")]
		[SerializeField] private EnemyAIData _enemyAI = new();

		[Header("Other")]
		public float StackVerticalOffset = 0.025f;
		public LayerMask InputMask;

		public int BaseHeroHealth => _baseHeroHealth;
		public int BaseEnemyHealth => _baseEnemyHealth;
		public int DeckSize => _deckSize;
		public int StartingHandSize => _startingHandSize;
		public HandLayoutData HandLayout => _handLayout;
		public BoardLayoutData BoardLayout => _boardLayout;
		public AnimationTimingData AnimationTiming => _animationTiming;
		public GameBalanceData GameBalance => _gameBalance;
		public CardGenerationData CardGeneration => _cardGeneration;
		public EnemyAIData EnemyAI => _enemyAI;
	}

	[Serializable]
	public class HandLayoutData
	{
		[Tooltip("Горизонтальное расстояние между картами")]
		public float HorizontalSpacing = 0.3f;

		[Tooltip("Вертикальная кривая раскладки")]
		public float VerticalCurve = 0.05f;

		[Tooltip("Глубина между картами")]
		public float DepthSpacing = 0.02f;

		[Tooltip("Угол поворота на карту")]
		public float AnglePerCard = 5f;
	}

	[Serializable]
	public class BoardLayoutData
	{
		[Tooltip("Расстояние между слотами")]
		public Vector2 Spacing = new(2.2f, 0);

		[Tooltip("Позиция героя")]
		public Vector3 HeroOrigin = new(0, 0, -2f);

		[Tooltip("Позиция врага")]
		public Vector3 EnemyOrigin = new(0, 0, 2f);

		[Tooltip("Поворот слотов (Euler углы)")]
		public Vector3 SlotRotation = new(90f, 0f, 0f);

		[Tooltip("Цвет слотов героя")]
		public Color HeroSlotColor = new(0.3f, 0.7f, 1f, 0.5f);

		[Tooltip("Цвет слотов врага")]
		public Color EnemySlotColor = new(1f, 0.3f, 0.3f, 0.5f);
	}

	[Serializable]
	public class AnimationTimingData
	{
		[Header("Attack")]
		[Tooltip("Задержка между атаками")]
		public float DelayBetweenAttacks = 1f;

		[Tooltip("Задержка после атаки")]
		public float PostAttackDelay = 1f;

		[Header("Turn")]
		[Tooltip("Задержка перед ходом врага")]
		public float EnemyTurnDelay = 1f;

		[Header("Cards")]
		[Tooltip("Задержка между картами при добавлении")]
		public float DelayBetweenCards = 0.5f;

		[Tooltip("Длительность движения карты")]
		public float CardMoveDuration = 0.25f;

		[Tooltip("Длительность обновления раскладки")]
		public float LayoutUpdateDuration = 0.2f;
	}

	[Serializable]
	public class GameBalanceData
	{
		[Tooltip("Максимум карт в руке")]
		public int MaxHandSize = 3;

		[Tooltip("Максимум карт, выложенных за ход")]
		public int MaxCardsPlacedPerTurn = 1;
	}

	[Serializable]
	public class CardGenerationData
	{
		[Tooltip("Диапазон HP для генерации карт")]
		public Vector2Int HpRange = new(1, 4);

		[Tooltip("Диапазон урона для генерации карт")]
		public Vector2Int DamageRange = new(1, 3);

		[Tooltip("Диапазон случайного поворота карт в стопке")]
		public Vector2 RotationRange = new(-25f, 25f);
	}

	[Serializable]
	public class EnemyAIData
	{
		[Tooltip("Задержка перед действиями врага")]
		public float ThinkDelay = 1f;
	}
}

