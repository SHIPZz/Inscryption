using System.Collections.Generic;
using Code.Features.UI;
using Code.Features.UI.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.Game.Systems
{
    //todo refactor this
    public class ProcessGameEndRequestSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new(1);
        private readonly List<GameEntity> _gameRequestsBuffer = new(5);
        private readonly IUIProvider _uiProvider;
        private readonly IGroup<GameEntity> _gameRequests;

        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;

        public ProcessGameEndRequestSystem(GameContext game, IUIProvider uiProvider)
        {
            _uiProvider = uiProvider;
            _requests = game.GetGroup(GameMatcher.GameEndRequest);
            _gameRequests = game.GetGroup(GameMatcher.AnyOf(
                GameMatcher.SwitchTurnRequest,
                GameMatcher.EndTurnRequest,
                GameMatcher.AttackRequest));

            _heroes = game.GetGroup(GameMatcher.Hero);
            _enemies = game.GetGroup(GameMatcher.Enemy);
        }

        public void Execute()
        {
            foreach (GameEntity request in _requests.GetEntities(_buffer))
            {
                DestroyGameRequests();

                foreach (var hero in _heroes)
                foreach (var enemy in _enemies)
                {
                    UpdateUI(request, hero, enemy);
                }

                request.Destroy();
            }
        }

        private void DestroyGameRequests()
        {
            foreach (GameEntity gameRequest in _gameRequests.GetEntities(_gameRequestsBuffer))
            {
                gameRequest.Destroy();
            }
        }

        private void UpdateUI(GameEntity request, GameEntity hero, GameEntity enemy)
        {
            bool heroWon = request.gameEndRequest.HeroWon;
            int heroHp = request.gameEndRequest.HeroHp;
            int enemyHp = request.gameEndRequest.EnemyHp;

            Debug.Log(
                $"[ProcessGameEndRequestSystem] Game ended - Hero Won: {heroWon}, Hero HP: {heroHp}, Enemy HP: {enemyHp}");
            _uiProvider.GameHUD.ShowGameEnd(heroWon, hero.Hp, enemy.Hp);
        }
    }
}