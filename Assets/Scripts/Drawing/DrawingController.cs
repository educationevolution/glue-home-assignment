using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    public class DrawingController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private DrawingLine _linePrefab;
        [SerializeField] private Transform _linesContainer;
        private Dictionary<int, DrawingLine> _drawingLinesById = new();
        private int _currentLineId = 0;

        private bool _isButtonDown = false;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isButtonDown = true;
                var newLine = Instantiate(_linePrefab, _linesContainer);
                newLine.transform.position = GetMouseWorldPosition;
                _currentLineId += 1;
                _drawingLinesById[_currentLineId] = newLine;
                return;
            } 
            if (Input.GetMouseButtonUp(0))
            {
                _isButtonDown = false;
            }
            if (_isButtonDown)
            {
                _drawingLinesById[_currentLineId].AddPointIfPossible(GetMouseWorldPosition);
            }
        }

        private Vector3 GetMouseWorldPosition
        {
            get 
            {
                var mousePosition = Input.mousePosition;
                mousePosition.z += _camera.transform.position.z * -1;
                return _camera.ScreenToWorldPoint(mousePosition);
            }
        }
    }
}