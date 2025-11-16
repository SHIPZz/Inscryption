using Code.Common.Extensions;
using Code.Common.Services;
using UnityEngine;
using Zenject;

namespace Code.Features.Camera.Behaviours
{
    public class SmoothCameraLookAtTarget : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private bool _ignoreY = false;
        [SerializeField, Min(0.01f)] private float _smoothTime = 0.15f;
        [SerializeField, Range(-90f, 90f)] private float _xRotationLimit = 20f;

        [Inject] private ICameraProvider _cameraProvider;

        private void Awake()
        {
            if (_target == null)
                _target = transform;
        }

        private void LateUpdate()
        {
            UnityEngine.Camera camera = _cameraProvider?.MainCamera;
            if (camera == null || _target == null)
                return;

            Vector3 camPos = camera.transform.position;
            Vector3 targetPos = _target.position;
            Quaternion desired = targetPos.GetLookRotationTo(camPos, _ignoreY);

            Vector3 desiredEuler = desired.eulerAngles;
            float clampedX = Mathf.Clamp(NormalizeAngle(desiredEuler.x), -_xRotationLimit, _xRotationLimit);
            Quaternion clampedDesired = Quaternion.Euler(clampedX, desiredEuler.y, desiredEuler.z);

            float t = Mathf.Clamp01(Time.deltaTime / _smoothTime);
            _target.rotation = Quaternion.Slerp(_target.rotation, clampedDesired, t);
        }

        private float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }
    }
}
