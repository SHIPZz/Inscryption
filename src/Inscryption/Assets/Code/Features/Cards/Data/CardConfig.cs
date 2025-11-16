using System.Collections.Generic;
using UnityEngine;

namespace Code.Features.Cards.Data
{
	[CreateAssetMenu(fileName = "CardConfig", menuName = "Inscryption/Cards/Card Config")]
	public class CardConfig : ScriptableObject
	{
		[Header("Available Cards")]
		[SerializeField] private List<CardData> _cards;

		public IReadOnlyList<CardData> Cards => _cards;
	}
}

