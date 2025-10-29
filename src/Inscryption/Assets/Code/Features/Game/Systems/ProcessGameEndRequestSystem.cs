using System.Collections.Generic;
using Code.Features.UI.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Game.Systems
{
    public class ProcessGameEndRequestSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new(1);
        private readonly List<GameEntity> _gameRequestsBuffer = new(16);
        private readonly IUIProvider _uiProvider;
        private readonly IGroup<GameEntity> _allRequests;

        public ProcessGameEndRequestSystem(GameContext game, IUIProvider uiProvider)
        {
            _uiProvider = uiProvider;
            _requests = game.GetGroup(GameMatcher.AllOf(GameMatcher.GameEndRequest, GameMatcher.ProcessingAvailable));

            _allRequests = game.GetGroup(GameMatcher.AllOf(GameMatcher.Request)
                .NoneOf(GameMatcher.GameEndRequest));
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests.GetEntities(_buffer))
            {
                DestroyRequests();

                Debug.Log("@@@ ProcessGameEndRequestSystem");
                
                UpdateUI(request);

                request.Destroy();
            }
        }

        private void DestroyRequests()
        {
            foreach (GameEntity request in _allRequests.GetEntities(_gameRequestsBuffer))
            {
                request.Destroy();
            }
        }

        private void UpdateUI(GameEntity request)
        {
            bool heroWon = request.gameEndRequest.HeroWon;
            int heroHp = request.gameEndRequest.HeroHp;
            int enemyHp = request.gameEndRequest.EnemyHp;

            Debug.Log($"[ProcessGameEndRequestSystem] Game ended - Hero Won: {heroWon}, Hero HP: {heroHp}, Enemy HP: {enemyHp}");
            _uiProvider.GameHUD.ShowGameEnd(heroWon, heroHp, enemyHp);
        }
    }
}