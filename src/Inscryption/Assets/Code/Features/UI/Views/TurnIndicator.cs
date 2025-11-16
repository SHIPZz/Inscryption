using TMPro;
using UnityEngine;

namespace Code.Features.UI.Views
{
    public class TurnIndicator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _turnText;
        [SerializeField] private GameObject _heroIndicator;
        [SerializeField] private GameObject _enemyIndicator;
        [SerializeField] private string _heroTurnText = "YOUR TURN";
        [SerializeField] private string _enemyTurnText = "ENEMY TURN";
        [SerializeField] private string _victoryText = "VICTORY!";
        [SerializeField] private string _defeatText = "DEFEAT!";

        public void SetHeroTurn(bool isHeroTurn)
        {
            if (_heroIndicator != null)
                _heroIndicator.SetActive(isHeroTurn);

            if (_enemyIndicator != null)
                _enemyIndicator.SetActive(!isHeroTurn);

            if (_turnText != null)
                _turnText.text = isHeroTurn ? _heroTurnText : _enemyTurnText;
        }

        public void ShowGameEnd(bool heroWon)
        {
            if (_heroIndicator != null)
                _heroIndicator.SetActive(false);

            if (_enemyIndicator != null)
                _enemyIndicator.SetActive(false);

            if (_turnText != null)
                _turnText.text = heroWon ? _victoryText : _defeatText;
        }
    }
}