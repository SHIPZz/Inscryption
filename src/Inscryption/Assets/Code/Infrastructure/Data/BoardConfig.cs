using UnityEngine;

namespace Code.Infrastructure.Data
{
	[CreateAssetMenu(fileName = "BoardConfig", menuName = "Inscryption/Board Config")]
	public class BoardConfig : ScriptableObject
	{
		[Header("Hand Positions")]
		[Tooltip("Позиции карт в руке героя")]
		[SerializeField] private Vector3[] _heroHandPositions = new Vector3[5]
		{
			new Vector3(-4f, 0f, -2f),
			new Vector3(-2f, 0f, -2f),
			new Vector3(0f, 0f, -2f),
			new Vector3(2f, 0f, -2f),
			new Vector3(4f, 0f, -2f)
		};

		[Tooltip("Позиции карт в руке врага")]
		[SerializeField] private Vector3[] _enemyHandPositions = new Vector3[5]
		{
			new Vector3(-4f, 0f, 2f),
			new Vector3(-2f, 0f, 2f),
			new Vector3(0f, 0f, 2f),
			new Vector3(2f, 0f, 2f),
			new Vector3(4f, 0f, 2f)
		};

		[Header("Board Positions")]
		[Tooltip("Позиции слотов героя (4 линии)")]
		[SerializeField] private Vector3[] _heroSlotPositions = new Vector3[4]
		{
			new Vector3(-3f, 0f, -1f),
			new Vector3(-1f, 0f, -1f),
			new Vector3(1f, 0f, -1f),
			new Vector3(3f, 0f, -1f)
		};

		[Tooltip("Позиции слотов врага (4 линии)")]
		[SerializeField] private Vector3[] _enemySlotPositions = new Vector3[4]
		{
			new Vector3(-3f, 0f, 1f),
			new Vector3(-1f, 0f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(3f, 0f, 1f)
		};

		[Header("Card Stack Positions")]
		[Tooltip("Позиция стопки карт героя")]
		[SerializeField] private Vector3 _heroCardStackPosition = new Vector3(-6f, 0f, -2f);

		[Tooltip("Позиция стопки карт врага")]
		[SerializeField] private Vector3 _enemyCardStackPosition = new Vector3(6f, 0f, 2f);

		public Vector3 GetHeroHandPosition(int index) =>
			index >= 0 && index < _heroHandPositions.Length ? _heroHandPositions[index] : Vector3.zero;

		public Vector3 GetEnemyHandPosition(int index) =>
			index >= 0 && index < _enemyHandPositions.Length ? _enemyHandPositions[index] : Vector3.zero;

		public Vector3 GetHeroSlotPosition(int laneIndex) =>
			laneIndex >= 0 && laneIndex < _heroSlotPositions.Length ? _heroSlotPositions[laneIndex] : Vector3.zero;

		public Vector3 GetEnemySlotPosition(int laneIndex) =>
			laneIndex >= 0 && laneIndex < _enemySlotPositions.Length ? _enemySlotPositions[laneIndex] : Vector3.zero;

		public Vector3 GetHeroCardStackPosition() => _heroCardStackPosition;

		public Vector3 GetEnemyCardStackPosition() => _enemyCardStackPosition;
	}
}
