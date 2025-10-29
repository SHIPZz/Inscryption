using Code.Common;
using Code.Common.Extensions;
using Code.Features.Hero.Services;
using UnityEngine;
using Zenject;

namespace Code.Features.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private HealthDisplay _healthDisplay;
        [SerializeField] private TurnIndicator _turnIndicator;
        [SerializeField] private EndTurnButton _endTurnButton;

        [Inject] private IHeroProvider _heroProvider;

        private bool _wasHeroTurn;

        private void Awake()
        {
            if (_endTurnButton != null)
                _endTurnButton.Initialize(OnEndTurnRequested);
        }

        private void OnEndTurnRequested()
        {
            GameEntity hero = _heroProvider.GetHero();
            if (hero == null || !hero.isHeroTurn)
                return;

            CreateEntity.Request()
                .With(x => x.isEndTurnRequest = true);
        }

        public void UpdateHeroHealth(int currentHp, int maxHp)
        {
            if (_healthDisplay != null)
                _healthDisplay.UpdateHeroHealth(currentHp, maxHp);
        }

        public void UpdateEnemyHealth(int currentHp, int maxHp)
        {
            if (_healthDisplay != null)
                _healthDisplay.UpdateEnemyHealth(currentHp, maxHp);
        }

        public void SetHeroTurn(bool isHeroTurn)
        {
            if (_turnIndicator != null)
                _turnIndicator.SetHeroTurn(isHeroTurn);

            UpdateEndTurnButton(isHeroTurn);
            _wasHeroTurn = isHeroTurn;
        }

        private void UpdateEndTurnButton(bool isHeroTurn)
        {
            if (_endTurnButton == null)
                return;

            if (!isHeroTurn)
            {
                _endTurnButton.Disable();
                return;
            }

            bool isTurnJustStarted = isHeroTurn && !_wasHeroTurn;
            if (isTurnJustStarted)
                _endTurnButton.Unlock();
        }

        public void ShowGameEnd(bool heroWon, int heroHp, int enemyHp)
        {
            if (_turnIndicator != null)
                _turnIndicator.ShowGameEnd(heroWon);

            if (_healthDisplay != null)
            {
                _healthDisplay.UpdateHeroHealth(heroHp, 20);
                _healthDisplay.UpdateEnemyHealth(enemyHp, 20);
            }

            if (_endTurnButton != null)
                _endTurnButton.Disable();

            Debug.Log($"[GameHUD] Showing game end - {(heroWon ? "VICTORY" : "DEFEAT")}");
        }
    }
}