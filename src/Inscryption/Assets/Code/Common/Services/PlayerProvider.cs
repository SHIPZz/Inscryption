using Entitas;

namespace Code.Common.Services
{
    public abstract class PlayerProvider
    {
        protected readonly GameContext Game;
        private readonly IMatcher<GameEntity> _playerMatcher;
        private readonly IMatcher<GameEntity> _activeMatcher;

        protected PlayerProvider(GameContext game, IMatcher<GameEntity> playerMatcher, IMatcher<GameEntity> activeMatcher)
        {
            Game = game;
            _playerMatcher = playerMatcher;
            _activeMatcher = activeMatcher;
        }

        public GameEntity GetPlayer()
        {
            foreach (GameEntity entity in Game.GetEntities(_playerMatcher))
            {
                if (!entity.isDestructed)
                    return entity;
            }
            return null;
        }

        public GameEntity GetActivePlayer()
        {
            foreach (GameEntity entity in Game.GetEntities(_playerMatcher))
            {
                if (_activeMatcher.Matches(entity) && !entity.isDestructed)
                    return entity;
            }
            return null;
        }

        public bool IsPlayerActive()
        {
            return GetActivePlayer() != null;
        }
    }
}

