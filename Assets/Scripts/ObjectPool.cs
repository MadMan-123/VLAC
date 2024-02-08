using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Deus
{
    public class ObjectPool
    {
        private List<GameObject> objects = new List<GameObject>();
        private GameObject prefab;

        private Transform pTransform;
        // Constructor to initialize the pool with a prefab, initial size, and parent transform
        public ObjectPool(GameObject prefab, int initialSize, Transform parent)
        {
            this.prefab = prefab;
            pTransform = parent;
            // Create the initial pool of objects
            for (int i = 0; i < initialSize; i++)
            {
                CreateObjectInPool();
            }
            
            Debug.Log($"Pool Created: {prefab.name}, Pool Size: {objects.Count}");
        }

        // Create a new object in the pool and add it to the list
        private GameObject CreateObjectInPool()
        {
            GameObject obj = Object.Instantiate(prefab, pTransform.position, Quaternion.identity, pTransform);
            obj.SetActive(false);
            // Give the object the "Pooled" tag
            obj.tag = "Pooled";
            objects.Add(obj);
            return obj;
        }

        // Get an inactive object from the pool
        public GameObject GetObject(Transform parent)
        {
            foreach (var obj in objects)
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    obj.transform.position = parent.position;
                    return obj;
                }
            }

            // If all objects are in use, create a new one and add it to the pool
            GameObject newObj = CreateObjectInPool();
            Debug.Log($"Pool Added: {newObj.name}, Pool Size: {objects.Count} : +1");
            newObj.SetActive(true);
            return newObj;
        }

        // Return an object to the pool by deactivating it
        public void ReturnObject(GameObject obj)
        {
            obj.transform.position = pTransform.position;

            obj.SetActive(false);
        }

        // Clean up function (not implemented in the provided code)
        void CleanUp()
        {
            objects.Clear();
            
            GC.Collect();
        }
    }
}
