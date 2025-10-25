using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        
        public Button EndTurnButton => _endTurnButton;

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
            Debug.Log("[GameHUD] End Turn button clicked");
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
    }
}
