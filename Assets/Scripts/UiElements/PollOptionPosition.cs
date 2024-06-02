using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UiElements
{
    public enum PollOptionPositionCategory
    {
        TwoOptions,
        ThreeOptions,
    }

    public class PollOptionPosition : MonoBehaviour
    {

        [SerializeField] private PollOptionPositionCategory _category;

        public PollOptionPositionCategory Category => _category;

        private Color GetCategoryColor()
        {
            switch (_category)
            {
                case PollOptionPositionCategory.TwoOptions:
                    return Color.yellow;
                case PollOptionPositionCategory.ThreeOptions:
                    return Color.red;
            }
            throw new Exception($"Unhandled {nameof(PollOptionPositionCategory)} {_category}!");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = GetCategoryColor();
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}