using Infrastructure;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UiElements
{
    public class PollOptionResultUi : PooledObject
    {
        const float MAX_ANIMATION_TIME = 2f;

        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private TextMeshProUGUI _resultPercentText;
        [SerializeField] private RectTransform _mainImageContainer;
        [SerializeField] private Image _mainImage;
        [SerializeField] private TextMeshProUGUI _yourChoiceText;
        [SerializeField] private RectTransform _voterAvatarsContainer;
        [SerializeField] private RectTransform _winningChoiceEffectsContainer;
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
        private Coroutine _animationCoroutine;

        private void Awake()
        {
            
        }

        
        public void DisplayResult(float ratio01, bool isWinningOption, bool isUserChoice)
        {
            _winningChoiceEffectsContainer.gameObject.SetActive(isWinningOption);
            _yourChoiceText.gameObject.SetActive(isUserChoice);
            _votersBarImage.sprite = isUserChoice ?
                _votersBarUserChoiceSprite : _votersBarDefaultSprite;
            _animationCoroutine = StartCoroutine(DisplayResultCoroutine(ratio01));
        }

        private IEnumerator DisplayResultCoroutine(float ratio01)
        {
            SetVotersBarHeight(_initialVotersBarHeight);
            var targetVotersBarHeightAddon = _maxVotersBarHeightAddon * ratio01;
            var currentAnimationDuration = MAX_ANIMATION_TIME * ratio01;
            var animationStartTime = Time.time;
            var animationEndTime = animationStartTime + currentAnimationDuration;
            var votePct = 0;
            _resultPercentText.text = $"{votePct}%";
            while (Time.time < animationEndTime)
            {
                var animationProgressRatio01 = (Time.time - animationStartTime) / currentAnimationDuration;
                var votersBarHeight = _initialVotersBarHeight + targetVotersBarHeightAddon * animationProgressRatio01;
                SetVotersBarHeight(votersBarHeight);
                var newVotePct = Mathf.CeilToInt(animationProgressRatio01 * 100);
                if (votePct != newVotePct)
                {
                    votePct = newVotePct;
                    _resultPercentText.text = $"{votePct}%";
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