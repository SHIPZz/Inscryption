using Code.Features.Hero.Services;
using Entitas;
using UnityEngine;

namespace Code.Features.UI.Systems
{
    public class ProcessEndTurnButtonSystem : IInitializeSystem
    {
        private readonly MetaContext _meta;
        private readonly GameContext _game;
        private readonly IHeroProvider _heroProvider;

        public ProcessEndTurnButtonSystem(MetaContext meta, GameContext game, IHeroProvider heroProvider)
        {
            _meta = meta;
            _game = game;
            _heroProvider = heroProvider;
        }

        public void Initialize()
        {
            if (!_meta.hasGameHUD)
                return;

            GameHUD hud = _meta.gameHUD.Value;
            if (hud == null || hud.EndTurnButton == null)
                return;

            hud.EndTurnButton.onClick.AddListener(OnEndTurnClicked);
        }

        private void OnEndTurnClicked()
        {
            GameEntity hero = _heroProvider.GetHero();
            if (hero == null || !hero.isHeroTurn)
            {
                Debug.LogWarning("[ProcessEndTurnButtonSystem] Not hero's turn!");
                return;
            }

            Debug.Log("[ProcessEndTurnButtonSystem] Creating EndTurnRequest");
            _game.CreateEntity().isEndTurnRequest = true;
        }
    }
}
