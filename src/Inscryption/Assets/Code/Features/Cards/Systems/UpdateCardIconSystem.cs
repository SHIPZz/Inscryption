using System.Collections.Generic;
using Entitas;

namespace Code.Features.Cards.Systems
{
	public class UpdateCardIconSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _cards;
		private readonly List<GameEntity> _buffer = new(32);

		public UpdateCardIconSystem(GameContext game)
		{
			_cards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Card,
					GameMatcher.CardIcon,
					GameMatcher.View));
		}

		public void Execute()
		{
			foreach (GameEntity card in _cards.GetEntities(_buffer))
			{
				if (card.View is CardEntityView cardView)
				{
					cardView.SetIcon(card.CardIcon);
				}
			}
		}
	}
}

