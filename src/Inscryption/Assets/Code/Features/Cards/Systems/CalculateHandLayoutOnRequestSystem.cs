using Code.Common;
using Code.Common.Extensions;
using Code.Features.Cards.Services;
using Code.Features.Layout.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Cards.Systems
{
    public class CalculateHandLayoutOnRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IHandLayoutService _handLayoutService;
        private readonly IGroup<GameEntity> _requests;
        private readonly System.Collections.Generic.List<GameEntity> _buffer = new(32);

        public CalculateHandLayoutOnRequestSystem(GameContext game, IHandLayoutService handLayoutService)
        {
            _game = game;
            _handLayoutService = handLayoutService;
            _requests = game.GetGroup(GameMatcher.UpdateHandLayoutRequest);
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests.GetEntities(_buffer))
            {
                int playerId = request.updateHandLayoutRequest.PlayerId;
                GameEntity player = _game.GetEntityWithId(playerId);

                if (IsValidPlayer(player))
                {
                    CreateCardLayoutRequests(player);
                }

                request.Destroy();
            }
        }

        private bool IsValidPlayer(GameEntity player)
        {
            return player != null && player.hasCardsInHand && player.CardsInHand.Count > 0;
        }

        private void CreateCardLayoutRequests(GameEntity player)
        {
            var parent = _handLayoutService.GetCardParent(player);

            if (parent == null)
            {
                Debug.LogWarning($"[CalculateHandLayoutSystem] Card parent is null for player {player.Id}");
                return;
            }

            CardLayoutData[] layoutData = _handLayoutService.CalculateLayout(player);

            for (int i = 0; i < player.CardsInHand.Count; i++)
            {
                int cardId = player.CardsInHand[i];
                GameEntity card = _game.GetEntityWithId(cardId);

                if (!IsValidCard(card))
                    continue;

                CardLayoutData layout = layoutData[i];

                CreateEntity
                    .Request()
                    .AddAnimateCardPositionRequest(cardId, layout.Position, layout.Rotation)
                    ;
            }

            string playerName = player.isHero ? "Hero" : "Enemy";
            Debug.Log($"[CalculateHandLayoutSystem] Created layout requests for {player.CardsInHand.Count} {playerName} cards");
        }

        private bool IsValidCard(GameEntity card)
        {
            return card != null && card.hasTransform && card.isInHand;
        }
    }
}
