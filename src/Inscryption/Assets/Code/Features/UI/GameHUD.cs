using Code.Features.Hero.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Features.UI
{
    //todo refactor this:
    public class GameHUD : MonoBehaviour
    {
        [Header("Health Display")] [SerializeField]
        private TextMeshProUGUI _heroHealthText;

        [SerializeField] private TextMeshProUGUI _enemyHealthText;

        [Header("Turn Indicator")] [SerializeField]
        private TextMeshProUGUI _turnIndicatorText;

        [SerializeField] private GameObject _heroTurnIndicator;
        [SerializeField] private GameObject _enemyTurnIndicator;

        [Header("End Turn Button")] [SerializeField]
        private Button _endTurnButton;

        [SerializeField] private TextMeshProUGUI _endTurnButtonText;
        [Inject] IHeroProvider _heroProvider;
        [Inject] private GameContext _gameContext;

        // Lock state: once player clicks End Turn, keep the button disabled until the next hero turn actually starts
        private bool _endTurnLockedUntilNextHeroTurn;
        private bool _lastTurnKnown;
        private bool _lastIsHeroTurn;

        private void Awake()
        {
            if (_endTurnButton != null)
            {
                _endTurnButton.onClick.AddListener(OnEndTurnClicked);
            }

            _endTurnLockedUntilNextHeroTurn = false;
            _lastTurnKnown = false;
            _lastIsHeroTurn = false;
        }

        private void OnDestroy()
        {
            if (_endTurnButton != null)
            {
                _endTurnButton.onClick.RemoveListener(OnEndTurnClicked);
            }
        }

        private void OnEndTurnClicked()
        {
            GameEntity hero = _heroProvider.GetHero();
            if (hero == null || !hero.isHeroTurn)
            {
                return;
            }

            _endTurnLockedUntilNextHeroTurn = true;
            if (_endTurnButton != null)
                _endTurnButton.interactable = false; // disable immediately until next player turn
            _gameContext.CreateEntity().isEndTurnRequest = true;
        }

        public void UpdateHeroHealth(int currentHp, int maxHp)
        {
            if (_heroHealthText != null)
            {
                _heroHealthText.text = $"Hero HP: {currentHp}/{maxHp}";
            }
        }

        public void UpdateEnemyHealth(int currentHp, int maxHp)
        {
            if (_enemyHealthText != null)
            {
                _enemyHealthText.text = $"Enemy HP: {currentHp}/{maxHp}";
            }
        }

        public void SetHeroTurn(bool isHeroTurn)
        {
            if (_heroTurnIndicator != null)
                _heroTurnIndicator.SetActive(isHeroTurn);
            if (_enemyTurnIndicator != null)
                _enemyTurnIndicator.SetActive(!isHeroTurn);
            if (_turnIndicatorText != null)
                _turnIndicatorText.text = isHeroTurn ? "YOUR TURN" : "ENEMY TURN";

            if (_endTurnButton != null)
            {
                // If it's not hero turn, always keep the button disabled
                if (!isHeroTurn)
                {
                    _endTurnButton.interactable = false;
                }
                else
                {
                    // isHeroTurn == true
                    bool isTurnJustStarted = !_lastTurnKnown || (_lastTurnKnown && !_lastIsHeroTurn && isHeroTurn);
                    if (isTurnJustStarted)
                    {
                        // New hero turn started: unlock and enable the button
                        _endTurnLockedUntilNextHeroTurn = false;
                        _endTurnButton.interactable = true;
                    }
                    else
                    {
                        // Still the same hero turn: respect the lock if it was set after click
                        _endTurnButton.interactable = !_endTurnLockedUntilNextHeroTurn;
                    }
                }
            }

            _lastIsHeroTurn = isHeroTurn;
            _lastTurnKnown = true;
        }

        public void ShowGameEnd(bool heroWon, int heroHp, int enemyHp)
        {
            if (_turnIndicatorText != null)
            {
                string result = heroWon ? "VICTORY!" : "DEFEAT!";
                _turnIndicatorText.text = result;
            }

            UpdateHeroHealth(heroHp, 20);
            UpdateEnemyHealth(enemyHp, 20);
            if (_heroTurnIndicator != null)
                _heroTurnIndicator.SetActive(false);
            if (_enemyTurnIndicator != null)
                _enemyTurnIndicator.SetActive(false);
            if (_endTurnButton != null)
                _endTurnButton.interactable = false;
            Debug.Log($"[GameHUD] Showing game end - {(heroWon ? "VICTORY" : "DEFEAT")}");
        }
    }
}