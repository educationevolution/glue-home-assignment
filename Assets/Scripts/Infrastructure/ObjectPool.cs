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

    /// <summary>
    /// Object pooling component.
    /// Note that usually object pools also include the following features:
    /// - Warm up instances when the app loads.
    /// - Add a limit to max instances per prefab.
    /// In this case I chose to only implement borrowing instances on demand 
    /// without a limit and without warming up instances in advance.
    /// Note that all borrowed instances are auto reverted before loading a new scene.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }
        private Dictionary<int, List<PooledObject>> _objectsPoolByPrefabInstanceId = new();
        private Dictionary<int, List<PooledObject>> _borrowedInstancesByObjectInstanceId = new();
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

        /// <summary>
        /// Returns a prefab instance from the pool.
        /// Returns an inactive instance if exists, otherwise creates a new instance.
        /// </summary>
        /// <param name="prefab">Instance prefab.</param>
        /// <param name="parent">Instance parent</param>
        /// <returns></returns>
        public PooledObject Borrow(PooledObject prefab, Transform parent)
        {
            var prefabInstanceId = prefab.GetInstanceID();
            if (_borrowedInstancesByObjectInstanceId.ContainsKey(prefabInstanceId) == false)
            {
                _borrowedInstancesByObjectInstanceId[prefabInstanceId] = new();
            }
            if (_objectsPoolByPrefabInstanceId.ContainsKey(prefabInstanceId) == false || _objectsPoolByPrefabInstanceId[prefabInstanceId].Count == 0)
            {
                var newInstance = Instantiate(prefab, parent);
                _borrowedInstancesByObjectInstanceId[prefabInstanceId].Add(newInstance);
                _prefabInstanceIdByObjectInstanceId[newInstance.GetInstanceID()] = prefabInstanceId;
                newInstance.HandlePostBorrowFromPool();
                return newInstance;
            }
            var existingInstance = _objectsPoolByPrefabInstanceId[prefabInstanceId][0];
            _objectsPoolByPrefabInstanceId[prefabInstanceId].RemoveAt(0);
            _borrowedInstancesByObjectInstanceId[prefabInstanceId].Add(existingInstance);
            existingInstance.transform.SetParent(parent);
            existingInstance.HandlePostBorrowFromPool();
            return existingInstance;
        }

        /// <summary>
        /// Reverts an instance back to the pool.
        /// </summary>
        /// <param name="pooledObject">Instance to return to pool</param>
        public void Revert(PooledObject pooledObject)
        {
            var prefabInstanceId = _prefabInstanceIdByObjectInstanceId[pooledObject.GetInstanceID()];
            pooledObject.HandlePreRevertToPool();
            pooledObject.transform.SetParent(transform);
            _borrowedInstancesByObjectInstanceId[prefabInstanceId].Remove(pooledObject);
            if (_objectsPoolByPrefabInstanceId.ContainsKey(prefabInstanceId) == false)
            {
                _objectsPoolByPrefabInstanceId[prefabInstanceId] = new();
            }
            _objectsPoolByPrefabInstanceId[prefabInstanceId].Add(pooledObject);
        }

        /// <summary>
        /// Reverts all borrowed instances to the pool.
        /// </summary>
        public void RevertAllInstancesToPool()
        {
            foreach (var prefabInstanceId in _borrowedInstancesByObjectInstanceId.Keys)
            {
                var borrowedInstances = _borrowedInstancesByObjectInstanceId[prefabInstanceId];
                for (var i = 0; i < borrowedInstances.Count; i++)
                {
                    Revert(borrowedInstances[i]);
                }
            }
            _borrowedInstancesByObjectInstanceId.Clear();
        }
    }
}