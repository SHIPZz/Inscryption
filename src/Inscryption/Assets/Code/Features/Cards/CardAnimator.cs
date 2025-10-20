using DG.Tweening;
using UnityEngine;

namespace Code.Features.Cards
{
	public class CardAnimator : MonoBehaviour
	{
		[SerializeField] private Transform _visualTransform;
		
		[Header("Selection Settings")]
		[SerializeField] private float _selectedScale = 1.2f;
		[SerializeField] private float _normalScale = 1f;
		[SerializeField] private float _selectionDuration = 0.2f;
		[SerializeField] private Ease _selectionEase = Ease.OutBack;

		private Tween _currentTween;

		private void Awake()
		{
			if (_visualTransform == null)
				_visualTransform = transform;
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

		private void OnDestroy()
		{
			_currentTween?.Kill();
		}
	}
}

