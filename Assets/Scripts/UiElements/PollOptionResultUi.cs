using Effects;
using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public struct PollOptionResultData
    {
        public float Ratio01;
        public bool IsWinningOption;
        public bool IsUserChoice;
        public string ImageUrl;
        public Vector3 PositionDeltaToOptionImage;
        public Vector2 ImageOriginSize;
    }

    public class PollOptionResultUi : PooledObject
    {
        const float MAX_ANIMATION_TIME = 1f;

        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private TextMeshProUGUI _resultPercentText;
        [SerializeField] private TextMeshProUGUI _yourChoiceText;
        [SerializeField] private RectTransform _voterAvatarsContainer;
        [SerializeField] private RectTransform _winningChoiceEffectsContainer;
        [Header("Main Image")]
        [SerializeField] private RectTransform _mainImageContainer;
        [SerializeField] private Image _mainImage;
        [SerializeField] private GenericUiElementAnimator _mainImageGenericAnimator;
        [Header("Voters Bar")]
        [SerializeField] private RectTransform _votersBar;
        [SerializeField] private Image _votersBarImage;
        [SerializeField] private Sprite _votersBarDefaultSprite;
        [SerializeField] private Sprite _votersBarUserChoiceSprite;
        [SerializeField] private float _initialVotersBarHeight = 300;
        [SerializeField] private float _maxVotersBarHeightAddon = 150;
#if UNITY_EDITOR
        [Header("Unity Editor")]
        [SerializeField] private Canvas _canvasForGizmos;
#endif
        public RectTransform RectTransform => _rootRectTransform;
        private Coroutine _animationCoroutine;
        private Vector3 _imageOriginPosition;

        private void Awake()
        {
            
        }

        public void DisplayResult(PollOptionResultData data)
        {
            _winningChoiceEffectsContainer.gameObject.SetActive(data.IsWinningOption);
            _yourChoiceText.gameObject.SetActive(data.IsUserChoice);
            _votersBarImage.sprite = data.IsUserChoice ?
                _votersBarUserChoiceSprite : _votersBarDefaultSprite;
            if (data.IsWinningOption)
            {
                const float WINNING_OPTION_SCALE = 1.3f;
                _mainImageGenericAnimator.AnimateToNewScale(WINNING_OPTION_SCALE);
            }
            _animationCoroutine = StartCoroutine(DisplayResultCoroutine(data.PositionDeltaToOptionImage, data.Ratio01));
            _mainImage.sprite = ClientServices.Instance.ImageStore.LoadImage(data.ImageUrl);
        }

        private IEnumerator DisplayResultCoroutine(Vector3 positionDeltaToOptionImage, float ratio01)
        {
            _imageOriginPosition = _mainImageContainer.position;
            positionDeltaToOptionImage.z = 0;
            _mainImageContainer.position += positionDeltaToOptionImage;
            SetVotersBarHeight(_initialVotersBarHeight);

            yield return new WaitForSeconds(0.2f);

            var deltaToTargetPosition = _imageOriginPosition - _mainImageContainer.position;
            const float MAX_DELTA_TO_END_ANIMATION = 0.1f;
            const float DELTA_MOVEMENT_MULTIPLIER = 0.1f;
            while (deltaToTargetPosition.magnitude > MAX_DELTA_TO_END_ANIMATION)
            {
                _mainImageContainer.position += deltaToTargetPosition * DELTA_MOVEMENT_MULTIPLIER;
                deltaToTargetPosition = _imageOriginPosition - _mainImageContainer.position;
                yield return null;
            }
            _mainImageContainer.position = _imageOriginPosition;

            yield return new WaitForSeconds(0.4f);

            var targetVotersBarHeightAddon = _maxVotersBarHeightAddon * ratio01;
            var currentAnimationDuration = MAX_ANIMATION_TIME * ratio01;
            var animationStartTime = Time.time;
            var animationEndTime = animationStartTime + currentAnimationDuration;
            var lastVotePct = 0;
            _resultPercentText.text = $"{lastVotePct}%";
            while (Time.time < animationEndTime)
            {
                var animationProgressRatio01 = (Time.time - animationStartTime) / currentAnimationDuration;
                var votersBarHeight = _initialVotersBarHeight + targetVotersBarHeightAddon * animationProgressRatio01;
                SetVotersBarHeight(votersBarHeight);
                var newVotePct = Mathf.CeilToInt(animationProgressRatio01 * 100);
                if (lastVotePct != newVotePct)
                {
                    lastVotePct = newVotePct;
                    _resultPercentText.text = $"{lastVotePct}%";
                }
                yield return null;
            }
            SetVotersBarHeight(_initialVotersBarHeight + targetVotersBarHeightAddon);
            _resultPercentText.text = $"{Mathf.CeilToInt(ratio01 * 100)}%";
        }

        private void SetVotersBarHeight(float height)
        {
            var sizeDelta = _votersBar.sizeDelta;
            sizeDelta.y = height;
            _votersBar.sizeDelta = sizeDelta;
        }

        public override void HandlePostBorrowFromPool()
        {
            SetVotersBarHeight(_initialVotersBarHeight);
        }

        public override void HandlePreRevertToPool()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }
            _mainImageContainer.position = _imageOriginPosition;
            _mainImageGenericAnimator.ResetAll();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_canvasForGizmos == null)
            {
                return;
            }
            Gizmos.color = Color.green;
            var position = transform.position + Vector3.up * _initialVotersBarHeight * _canvasForGizmos.transform.localScale.y;
            var cubeSize = new Vector3(10, 0.3f, 1);
            Gizmos.DrawCube(position, cubeSize);
            position += Vector3.up * _maxVotersBarHeightAddon * _canvasForGizmos.transform.localScale.y;
            Gizmos.DrawCube(position, cubeSize);
        }
#endif
    }
}