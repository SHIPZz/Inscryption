using System.Collections.Generic;
using Code.Features.UI;
using Entitas;
using UnityEngine;

namespace Code.Features.Game.Systems
{
    public class ProcessGameEndRequestSystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _requests;
        private readonly List<GameEntity> _buffer = new(2);
        private GameHUD _gameHUD;

        public ProcessGameEndRequestSystem(GameContext game)
        {
            _game = game;
            _requests = game.GetGroup(GameMatcher.GameEndRequest);
        }

        public void Execute()
        {
            if (_gameHUD == null)
            {
                _gameHUD = Object.FindObjectOfType<GameHUD>();
            }

            foreach (GameEntity request in _requests.GetEntities(_buffer))
            {
                bool heroWon = request.gameEndRequest.HeroWon;
                int heroHp = request.gameEndRequest.HeroHp;
                int enemyHp = request.gameEndRequest.EnemyHp;

                Debug.Log($"[ProcessGameEndRequestSystem] Game ended - Hero Won: {heroWon}, Hero HP: {heroHp}, Enemy HP: {enemyHp}");

                if (_gameHUD != null)
                {
                    _gameHUD.ShowGameEnd(heroWon, heroHp, enemyHp);
                }

                request.isDestructed = true;
            }
        }
    }
}
