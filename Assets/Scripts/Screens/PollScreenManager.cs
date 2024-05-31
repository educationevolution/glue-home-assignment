using Scripts.UiElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Screens
{
    public class PollScreenManager : MonoBehaviour
    {
        [SerializeField] private PollOptionUi _pollOptionUiPrefab;
        [SerializeField] private PollOptionPosition[] _pollOptionPositions;
        [SerializeField] private RectTransform _pollOptionsContainer;
        private Dictionary<int, List<PollOptionPosition>> _optionPositionsByOptionsCount;

        private void Awake()
        {
            _optionPositionsByOptionsCount = new();
            for (var i = 0; i < _pollOptionPositions.Length; i++) 
            {
                var optionPosition = _pollOptionPositions[i];
                var optionsCount = GetOptionCountByCategory(optionPosition.Category);
                if (_optionPositionsByOptionsCount.ContainsKey(optionsCount) == false)
                {
                    _optionPositionsByOptionsCount[optionsCount] = new();
                }
                _optionPositionsByOptionsCount[optionsCount].Add(optionPosition);
            }
            DisplayPoll();
        }

        private int GetOptionCountByCategory(PollOptionPositionCategory category) 
        {
            switch (category)
            {
                case PollOptionPositionCategory.TwoOptions:
                    return 2;
                case PollOptionPositionCategory.ThreeOptions:
                    return 3;
                case PollOptionPositionCategory.FourOptions:
                    return 4;
            }
            throw new Exception($"Unhandled {nameof(PollOptionPositionCategory)} {category}!");
        }

        public void DisplayPoll()
        {
            var optionsCount = 4;
            for (var i = 0; i < _optionPositionsByOptionsCount[optionsCount].Count; i++) 
            {
                var optionPosition = _optionPositionsByOptionsCount[optionsCount][i];
                var newOptionUi = Instantiate(_pollOptionUiPrefab, _pollOptionsContainer);
                newOptionUi.RectTransform.position = optionPosition.transform.position;
                newOptionUi.Initialize(new PollOptionUiInitializeData()
                {
                    Id = i,
                    Title = "TEXT"
                });
            }
        }
    }
}