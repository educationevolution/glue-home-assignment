using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class GenericUiElementAnimator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroupTransparency;
        [SerializeField] private AnimationCurve _bounceAnimationCurve;
        private float? _targetAlpha;
        private float? _targetScale;
        private float? _bounceStartTime;
        private float _bounceDuration;

        public void ResetAll()
        {
            _targetAlpha = null;
            if (_canvasGroupTransparency != null)
            {
                _canvasGroupTransparency.alpha = 1;
            }
            _targetScale = null;
            transform.localScale = Vector3.one;
        }

        public void AnimateToFullTransparency()
        {
            _targetAlpha = 0;
        }

        public void AnimateToNewScale(float scale)
        {
            _targetScale = scale;
        }

        public void Bounce(float duration = 0.4f)
        {
            _bounceStartTime = Time.time;
            _bounceDuration = duration;
        }

        private void Update()
        {
            AlphaUpdate();
            ScaleUpdate();
            BounceUpdate();
        }

        private void BounceUpdate()
        {
            if (_bounceStartTime == null)
            {
                return;
            }
            var bounceEndTime = _bounceStartTime + _bounceDuration;
            if (Time.time > bounceEndTime)
            {
                _bounceStartTime = null;
                SetLocalScale(1);
                return;
            }
            var proressRatio01 = (Time.time - _bounceStartTime.Value) / _bounceDuration;
            SetLocalScale(_bounceAnimationCurve.Evaluate(proressRatio01));
        }

        private void ScaleUpdate()
        {
            if (_targetScale == null)
            {
                return;
            }
            var delta = _targetScale.Value - transform.localScale.x;
            const float MAX_SCALE_DELTA_TO_END_ANIMATION = 0.01f;
            if (Mathf.Abs(delta) < MAX_SCALE_DELTA_TO_END_ANIMATION)
            {
                SetLocalScale(_targetScale.Value);
                _targetScale = null;
                return;
            }
            const float SCALE_CHANGE_PER_FRAME = 0.01f;
            var newScale = transform.localScale.x > _targetScale.Value ?
                transform.localScale.x - SCALE_CHANGE_PER_FRAME : transform.localScale.x + SCALE_CHANGE_PER_FRAME;
            SetLocalScale(newScale);
        }

        private void SetLocalScale(float scale)
        {
            var localScale = transform.localScale;
            localScale.x = scale;
            localScale.y = scale;
            transform.localScale = localScale;
        }

        private void AlphaUpdate()
        {
            if (_targetAlpha == null)
            {
                return;
            }
            var delta = _targetAlpha.Value - _canvasGroupTransparency.alpha;
            const float MAX_ALPHA_DELTA_TO_END_ANIMATION = 0.1f;
            if (Mathf.Abs(delta) < MAX_ALPHA_DELTA_TO_END_ANIMATION)
            {
                _canvasGroupTransparency.alpha = _targetAlpha.Value;
                _targetAlpha = null;
                return;
            }
            const float ALPHA_CHANGE_PER_FRAME = 0.1f;
            _canvasGroupTransparency.alpha += _targetAlpha.Value > _canvasGroupTransparency.alpha ?
                ALPHA_CHANGE_PER_FRAME : -ALPHA_CHANGE_PER_FRAME;
        }
    }
}