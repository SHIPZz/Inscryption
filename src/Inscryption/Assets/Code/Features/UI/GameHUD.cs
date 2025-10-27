using Code.Features.Hero.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace Code.Features.UI
{
    public class GameHUD : MonoBehaviour
    {
        [Header("Health Display")]
        [SerializeField] private TextMeshProUGUI _heroHealthText;
        [SerializeField] private TextMeshProUGUI _enemyHealthText;
        [Header("Turn Indicator")]
        [SerializeField] private TextMeshProUGUI _turnIndicatorText;
        [SerializeField] private GameObject _heroTurnIndicator;
        [SerializeField] private GameObject _enemyTurnIndicator;
        [Header("End Turn Button")]
        [SerializeField] private Button _endTurnButton;
        [SerializeField] private TextMeshProUGUI _endTurnButtonText;
        [Inject] IHeroProvider _heroProvider;
        [Inject] private GameContext _gameContext;
        private void Awake()
        {
            if (_endTurnButton != null)
            {
                _endTurnButton.onClick.AddListener(OnEndTurnClicked);
            }
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
                _endTurnButton.interactable = isHeroTurn;
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
