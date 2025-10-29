using Code.Common.Visuals;
using DG.Tweening;
using UnityEngine;

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
        [SerializeField] private float _selectedYOffset = 0.2f;
        [SerializeField] private float _selectionDuration = 0.2f;
        [SerializeField] private Ease _selectionEase = Ease.OutBack;

        [Header("Attack Animation Settings")] [SerializeField]
        private Color _attackColor = Color.red;

        [SerializeField] private float _attackColorDuration = 0.15f;
        [SerializeField] private float _attackDistanceMultiplier = 0.7f;
        [SerializeField] private float _attackMoveDuration = 0.2f;
        [SerializeField] private float _attackReturnDelay = 0.5f;
        [SerializeField] private float _attackReturnDuration = 0.3f;
        [SerializeField] private Ease _attackMoveEase = Ease.OutQuad;
        [SerializeField] private Ease _attackReturnEase = Ease.OutBack;

        [Header("Attack Shake Settings")] [SerializeField]
        private float _attackShakeDuration = 0.3f;

        [SerializeField] private float _attackShakeStrength = 0.2f;
        [SerializeField] private int _attackShakeVibrato = 10;
        
        private Tween _currentTween;
        private Tween _currentPositionTween;
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
            _currentPositionTween?.Kill();

            _currentTween = _visualTransform.DOScale(_selectedScale, _selectionDuration)
                .SetEase(_selectionEase);

            Vector3 targetPosition = _visualTransform.localPosition + _visualTransform.up * _selectedYOffset;
            _currentPositionTween = _visualTransform.DOLocalMove(targetPosition, _selectionDuration)
                .SetEase(_selectionEase);
        }

        public void Deselect()
        {
            if (_visualTransform == null)
                return;

            _currentTween?.Kill();
            _currentPositionTween?.Kill();

            _currentTween = _visualTransform.DOScale(_normalScale, _selectionDuration)
                .SetEase(_selectionEase);

            Vector3 targetPosition = _visualTransform.localPosition - _visualTransform.up * _selectedYOffset;
            _currentPositionTween = _visualTransform.DOLocalMove(targetPosition, _selectionDuration)
                .SetEase(_selectionEase);
        }

        public void PlayDamageAnimation()
        {
            if (_visualTransform != null)
            {
                _visualTransform.DOKill();
            }

            if (_renderer != null && _renderer.material != null)
            {
                Color from = _renderer.material.color;
                Sequence seq = DOTween.Sequence();
                seq.Append(_renderer.material.DOColor(_damageColor, _damageFlashDuration));
                seq.Append(_renderer.material.DOColor(_originalColor, _damageFlashDuration));
            }
        }

        public void PlayAttackAnimation(Transform target)
        {
            if (_visualTransform == null || _renderer == null || target == null)
                return;

            Vector3 originalPosition = _visualTransform.position;
            
            Vector3 direction = (target.position - originalPosition).normalized;
            Vector3 attackPosition = originalPosition + direction * _attackDistanceMultiplier;

            Sequence seq = DOTween.Sequence();

            seq.Append(_renderer.material.DOColor(_attackColor, _attackColorDuration));
            seq.Join(_visualTransform.DOMove(attackPosition, _attackMoveDuration).SetEase(_attackMoveEase));
            seq.AppendInterval(_attackReturnDelay);

            seq.Append(_visualTransform.DOMove(originalPosition, _attackReturnDuration).SetEase(_attackReturnEase));
            seq.Join(_renderer.material.DOColor(_originalColor, _attackReturnDuration));
        }

        private void OnDestroy()
        {
            _currentTween?.Kill();
            _currentPositionTween?.Kill();
        }
    }
}