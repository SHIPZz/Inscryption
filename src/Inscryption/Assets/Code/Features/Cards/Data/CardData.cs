using System;
using UnityEngine;

namespace Code.Features.Cards.Data
{
	[Serializable]
	public class CardData
	{
		[SerializeField] [Range(1, 4)] private int _hp = 2;
		
		[SerializeField] [Range(1, 3)] private int _damage = 1;

		[SerializeField] private CardVisualData _visualData;

		public int Hp => _hp;
		public int Damage => _damage;
		public CardVisualData VisualData => _visualData;
	}
}

