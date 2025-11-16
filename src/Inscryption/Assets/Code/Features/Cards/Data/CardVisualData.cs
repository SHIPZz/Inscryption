using System;
using UnityEngine;

namespace Code.Features.Cards.Data
{
	[Serializable]
	public class CardVisualData
	{
		[SerializeField] private Sprite _icon;

		public Sprite Icon => _icon;
	}
}

