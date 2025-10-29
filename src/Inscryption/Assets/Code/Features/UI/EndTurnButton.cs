using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Features.UI
{
    public class EndTurnButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _buttonText;

        private Action _onEndTurnClicked;
        private bool _isLocked;

        private void Awake()
        {
            if (_button != null)
                _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void Initialize(Action onEndTurnCallback)
        {
            _onEndTurnClicked = onEndTurnCallback;
        }

        private void OnButtonClicked()
        {
            if (_isLocked)
                return;

            Lock();
            _onEndTurnClicked?.Invoke();
        }

        public void Lock()
        {
            _isLocked = true;
            SetInteractable(false);
        }

        public void Unlock()
        {
            _isLocked = false;
            SetInteractable(true);
        }

        public void Disable()
        {
            SetInteractable(false);
        }

        private void SetInteractable(bool interactable)
        {
            if (_button != null)
                _button.interactable = interactable;
        }
    }
}
