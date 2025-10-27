using DG.Tweening;
using UnityEngine;
using Code.Features.Statuses.Services;

namespace Code.Features.Cards
{
    public class CardAnimator : MonoBehaviour, IDamageAnimator, IAttackAnimator
    {
        [SerializeField] private Transform _visualTransform;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _damageColor = Color.red;
        [SerializeField] private float _damageFlashDuration = 0.1f;
        [SerializeField] private float _damageShakeStrength = 10f;
        [SerializeField] private int _damageShakeVibrato = 15;
        [SerializeField] private float _damageShakeDuration = 0.2f;

        [Header("Selection Settings")] [SerializeField]
        private float _selectedScale = 1.2f;

        [SerializeField] private float _normalScale = 1f;
        [SerializeField] private float _selectionDuration = 0.2f;
        [SerializeField] private Ease _selectionEase = Ease.OutBack;

        [Header("Attack Animation Settings")] [SerializeField]
        private float _attackLiftHeight = 0.5f;

        [SerializeField] private float _attackThruDistance = 1f;
        [SerializeField] private float _attackLiftDuration = 0.2f;
        [SerializeField] private float _attackThruDuration = 0.3f;
        [SerializeField] private float _attackReturnDuration = 0.3f;
        [SerializeField] private Ease _attackLiftEase = Ease.OutQuad;
        [SerializeField] private Ease _attackThruEase = Ease.InOutQuad;
        [SerializeField] private Ease _attackReturnEase = Ease.OutElastic;
        private Tween _currentTween;
        private Color _originalColor;
      
        public Transform VisualTransform => _visualTransform;

        private void Awake()
        {
            if (_visualTransform == null)
                _visualTransform = transform;
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();
            if (_renderer != null && _renderer.material != null)
                _originalColor = _renderer.material.color;
        }

        public void Select()
        {
            if (_visualTransform == null)
                return;
            _currentTween?.Kill();
            _currentTween = _visualTransform.DOScale(_selectedScale, _selectionDuration)
                .SetEase(_selectionEase);
        }

        public void Deselect()
        {
            if (_visualTransform == null)
                return;
            _currentTween?.Kill();
            _currentTween = _visualTransform.DOScale(_normalScale, _selectionDuration)
                .SetEase(_selectionEase);
        }

        public void PlayDamageAnimation()
        {
            if (_visualTransform != null)
            {
                _visualTransform.DOKill();
                _visualTransform.DOShakeRotation(_damageShakeDuration, _damageShakeStrength, _damageShakeVibrato);
            }

            if (_renderer != null && _renderer.material != null)
            {
                Color from = _renderer.material.color;
                Sequence seq = DOTween.Sequence();
                seq.Append(_renderer.material.DOColor(_damageColor, _damageFlashDuration));
                seq.Append(_renderer.material.DOColor(_originalColor, _damageFlashDuration));
            }
        }

        public void PlayAttackAnimation(int targetId)
        {
            if (transform == null)
                return;
            Sequence seq = DOTween.Sequence();
            seq.Append(_renderer.material.DOColor(Color.chartreuse, _damageFlashDuration));
            seq.Append(_renderer.material.DOColor(_originalColor, _damageFlashDuration));
        }

        private void OnDestroy()
        {
            _currentTween?.Kill();
        }
    }
}