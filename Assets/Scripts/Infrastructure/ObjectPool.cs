using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    public abstract class PooledObject : MonoBehaviour
    {
        public abstract void PrepareForUsage();
        public abstract void PreRevertToPool();
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

        public PooledObject GetObjectFromPool(PooledObject prefab, Transform parent)
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
                newInstance.PrepareForUsage();
                return newInstance;
            }
            var existingInstance = _inactivePool[prefabInstanceId][0];
            _inactivePool[prefabInstanceId].RemoveAt(0);
            existingInstance.transform.SetParent(parent);
            existingInstance.PrepareForUsage();
            return existingInstance;
        }

        public void RevertObjectToPool(PooledObject pooledObject)
        {
            var prefabInstanceId = _prefabInstanceIdByObjectInstanceId[pooledObject.GetInstanceID()];
            pooledObject.PreRevertToPool();
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