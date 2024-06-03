using Effects;
using Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public struct PollOptionResultData
    {
        /// <summary>
        /// A value between 0-1 (1=100%), representing the voting score for this option.
        /// </summary>
        public float ResultRatio01;
        public List<string> VotersAvatarImageUrls;
        public bool IsWinningOption;
        public bool IsUserChoice;
        public string ImageUrl;
        /// <summary>
        /// The delta between the option's image to the result component's image.
        /// This value is needed to animate the image in this component, from the option
        /// component to the center of this component.
        /// </summary>
        public Vector3 PositionDeltaToOptionImage;
        public Vector2 ImageOriginSize;
    }

    /// <summary>
    /// This component displays and animates a single poll result.
    /// </summary>
    public class PollOptionResultUi : PooledObject
    {
        const float MAX_ANIMATION_TIME = 1f;

        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private TextMeshProUGUI _resultPercentText;
        [SerializeField] private TextMeshProUGUI _yourChoiceText;
        [SerializeField] private RectTransform _voterAvatarsContainer;
        [SerializeField] private RectTransform _winningChoiceEffectsContainer;
        [SerializeField] private PollVoterAvatarImageUi _voterAvatarImagePrefab;
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
        [Header("Add Avatar Image Button")]
        [SerializeField] private Button _addAvatarImageButton;
        [SerializeField] private GenericUiElementAnimator _addAvatarImageGenericAnimator;
#if UNITY_EDITOR
        [Header("Unity Editor")]
        [SerializeField] private Canvas _canvasForGizmos;
#endif
        public RectTransform RectTransform => _rootRectTransform;
        public Action OnAddAvatarImageClicked;
        private Coroutine _animationCoroutine;
        private Vector3 _imageOriginPosition;
        private float _maxVotersBarHeightAddon;

        private void Awake()
        {
            _addAvatarImageButton.onClick.AddListener(AddAvatarImageClickedCallback);
        }

        /// <summary>
        /// Display and animate poll result.
        /// </summary>
        public void DisplayResult(PollOptionResultData data)
        {
            // Calculating voting bar's max height based on the parent container position
            _maxVotersBarHeightAddon = _rootRectTransform.anchoredPosition.y * -1 - _initialVotersBarHeight;

            _winningChoiceEffectsContainer.gameObject.SetActive(data.IsWinningOption);
            _yourChoiceText.gameObject.SetActive(data.IsUserChoice);
            _votersBarImage.sprite = data.IsUserChoice ?
                _votersBarUserChoiceSprite : _votersBarDefaultSprite;
            if (data.IsWinningOption)
            {
                const float WINNING_OPTION_SCALE = 1.3f;
                _mainImageGenericAnimator.AnimateToNewScale(WINNING_OPTION_SCALE);
            }

            var children = _voterAvatarsContainer.GetComponentsInChildren<PollVoterAvatarImageUi>();
            for (var i = 0; i < children.Length; i++)
            {
                Destroy(children[i].gameObject);
            }
            for (var i = 0; i < data.VotersAvatarImageUrls.Count; i++)
            {
                var avatarImage = ObjectPool.Instance.Borrow(_voterAvatarImagePrefab, _voterAvatarsContainer).GetComponent<PollVoterAvatarImageUi>();
                avatarImage.ShowImage(data.VotersAvatarImageUrls[i]);
            }

            _addAvatarImageButton.gameObject.SetActive(false);

            _animationCoroutine = StartCoroutine(DisplayResultCoroutine(data.PositionDeltaToOptionImage, data.ResultRatio01,
                activateAvatarImageButton: data.IsUserChoice));
            _mainImage.sprite = ClientServices.Instance.ImageStore.LoadImage(data.ImageUrl);
            _resultPercentText.text = "0%";
        }

        private IEnumerator DisplayResultCoroutine(Vector3 positionDeltaToOptionImage, float ratio01, 
            bool activateAvatarImageButton)
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
            _resultPercentText.text = $"{Mathf.RoundToInt(ratio01 * 100)}%";

            if (activateAvatarImageButton)
            {
                _addAvatarImageButton.gameObject.SetActive(true);
                _addAvatarImageButton.transform.localScale = Vector3.zero;
                _addAvatarImageGenericAnimator.Bounce();
            }
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

        private void AddAvatarImageClickedCallback()
        {
            OnAddAvatarImageClicked?.Invoke();
        }
    }
}