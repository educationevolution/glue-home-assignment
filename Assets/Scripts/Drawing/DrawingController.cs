using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infrastructure
{
    public class DrawingController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private DrawingLine _linePrefab;
        [SerializeField] private Transform _linesContainer;
        [SerializeField] private RectTransform _drawingBounds;
        private List<DrawingLine> _drawingLines = new();
        public bool IsEnabled { get; private set; } = false;
        private bool _isButtonDown = false;
        private Rect _drawingBoundsRect;

        private void Awake()
        {
            var xMin = _drawingBounds.anchorMin.x * Screen.width;
            var yMin = _drawingBounds.anchorMin.y * Screen.height;
            _drawingBoundsRect = new()
            {
                x = xMin,
                y = yMin,
                width = (_drawingBounds.anchorMax.x - _drawingBounds.anchorMin.x) * Screen.width,
                height = (_drawingBounds.anchorMax.y - _drawingBounds.anchorMin.y) * Screen.height
            };
        }

        public void SetIsEnabled(bool isEnabled)
        {
            IsEnabled = isEnabled;
            if (IsEnabled == false)
            {
                _isButtonDown = false;
            }
        }

        public void Clear()
        {
            for (var i = 0; i < _drawingLines.Count; i++) 
            {
                ObjectPool.Instance.Revert(_drawingLines[i]);
            }
            _drawingLines.Clear();
        }

        private void Update()
        {
            if (IsEnabled == false)
            {
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                _isButtonDown = true;
                var newLine = ObjectPool.Instance.Borrow(_linePrefab, _linesContainer).GetComponent<DrawingLine>();
                newLine.Initialize(GetMouseWorldPosition);
                _drawingLines.Add(newLine);
                return;
            } 
            if (Input.GetMouseButtonUp(0))
            {
                _isButtonDown = false;
            }
            if (_isButtonDown)
            {
                _drawingLines.Last().AddPointIfPossible(GetMouseWorldPosition);
            }
        }

        private Vector3 GetMouseWorldPosition
        {
            get 
            {
                var mousePosition = Input.mousePosition;
                var inBoundsPosition = mousePosition;
                inBoundsPosition.x = Mathf.Max(inBoundsPosition.x, _drawingBoundsRect.x);
                inBoundsPosition.y = Mathf.Max(inBoundsPosition.y, _drawingBoundsRect.y);
                inBoundsPosition.x = Mathf.Min(inBoundsPosition.x, _drawingBoundsRect.x + _drawingBoundsRect.width);
                inBoundsPosition.y = Mathf.Min(inBoundsPosition.y, _drawingBoundsRect.y + _drawingBoundsRect.height);
                inBoundsPosition.z += _camera.transform.position.z * -1;
                return _camera.ScreenToWorldPoint(inBoundsPosition);
            }
        }
    }
}