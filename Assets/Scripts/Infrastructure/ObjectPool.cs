using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    public abstract class PooledObject : MonoBehaviour
    {
        public abstract void HandlePostBorrowFromPool();
        public abstract void HandlePreRevertToPool();
    }

    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }
        private Dictionary<int, List<PooledObject>> _inactivePool = new();
        private Dictionary<int, List<PooledObject>> _activePool = new();
        private Dictionary<int, int> _prefabInstanceIdByObjectInstanceId = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public PooledObject Borrow(PooledObject prefab, Transform parent)
        {
            var prefabInstanceId = prefab.GetInstanceID();
            if (_inactivePool.ContainsKey(prefabInstanceId) == false || _inactivePool[prefabInstanceId].Count == 0)
            {
                var newInstance = Instantiate(prefab, parent);
                if (_activePool.ContainsKey(prefabInstanceId) == false)
                {
                    _activePool[prefabInstanceId] = new();
                }
                _activePool[prefabInstanceId].Add(newInstance);
                _prefabInstanceIdByObjectInstanceId[newInstance.GetInstanceID()] = prefabInstanceId;
                newInstance.HandlePostBorrowFromPool();
                return newInstance;
            }
            var existingInstance = _inactivePool[prefabInstanceId][0];
            _inactivePool[prefabInstanceId].RemoveAt(0);
            existingInstance.transform.SetParent(parent);
            existingInstance.HandlePostBorrowFromPool();
            return existingInstance;
        }

        public void Revert(PooledObject pooledObject)
        {
            var prefabInstanceId = _prefabInstanceIdByObjectInstanceId[pooledObject.GetInstanceID()];
            pooledObject.HandlePreRevertToPool();
            pooledObject.transform.SetParent(transform);
            _activePool[prefabInstanceId].Remove(pooledObject);
            if (_inactivePool.ContainsKey(prefabInstanceId) == false)
            {
                _inactivePool[prefabInstanceId] = new();
            }
            _inactivePool[prefabInstanceId].Add(pooledObject);
        }
    }
}