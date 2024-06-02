using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    public class DrawingLine : PooledObject
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _minDistanceBetweenPoints = 0.1f;
        private bool _isAllowAddingPoints = false;

        public void Initialize(Vector3 initialPosition)
        {
            transform.position = initialPosition;
            _lineRenderer.positionCount = 1;
            _lineRenderer.SetPosition(0, transform.position);
            _isAllowAddingPoints = true;
        }

        public void AddPointIfPossible(Vector3 worldPosition)
        {
            if (_isAllowAddingPoints == false)
            {
                return;
            }
            worldPosition.z = transform.parent.position.z;
            var lastPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);
            var distanceFromLastPosition = Vector3.Distance(lastPosition, worldPosition);
            if (distanceFromLastPosition < _minDistanceBetweenPoints)
            {
                return;
            }
            _lineRenderer.positionCount += 1;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, worldPosition);
        }

        public override void HandlePostBorrowFromPool()
        {
            _lineRenderer.positionCount = 0;
        }

        public override void HandlePreRevertToPool()
        {
            _lineRenderer.positionCount = 0;
            _isAllowAddingPoints = false;
        }
    }
}