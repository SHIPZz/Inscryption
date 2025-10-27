using System.Collections.Generic;
using Code.Features.Hero.Services;
using Entitas;

namespace Code.Features.Input.Systems
{
    public class ProcessCardClickRequestSystem : IExecuteSystem
    {
        private readonly InputContext _input;
        private readonly GameContext _game;
        private readonly IGroup<InputEntity> _requests;
        private readonly List<InputEntity> _buffer = new();
        private readonly IHeroProvider _heroProvider;

        public ProcessCardClickRequestSystem(InputContext input, GameContext game, IHeroProvider heroProvider)
        {
            _heroProvider = heroProvider;
            _input = input;
            _game = game;
            _requests = _input.GetGroup(InputMatcher.CardClickRequest);
        }

        public void Execute()
        {
            foreach (InputEntity request in _requests.GetEntities(_buffer))
            {
                int cardId = request.CardClickRequest;
                GameEntity card = _game.GetEntityWithId(cardId);

                GameEntity activeHero = _heroProvider.GetActiveHero();
                if (card != null && card.isCard && card.hasCardOwner && activeHero != null && card.CardOwner == activeHero.Id)
                {
                    card.isSelected = !card.isSelected;
                }

                request.Destroy();
            }
        }
    }
}

