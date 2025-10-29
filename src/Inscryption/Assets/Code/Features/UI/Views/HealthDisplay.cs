using TMPro;
using UnityEngine;

namespace Code.Features.UI.Views
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _heroHealthText;
        [SerializeField] private TextMeshProUGUI _enemyHealthText;

        public void UpdateHeroHealth(int currentHp, int maxHp)
        {
            if (_heroHealthText != null)
                _heroHealthText.text = $"Hero HP: {currentHp}/{maxHp}";
        }

        public void UpdateEnemyHealth(int currentHp, int maxHp)
        {
            if (_enemyHealthText != null)
                _enemyHealthText.text = $"Enemy HP: {currentHp}/{maxHp}";
        }
    }
}
