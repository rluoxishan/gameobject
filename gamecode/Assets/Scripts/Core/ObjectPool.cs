using System.Collections.Generic;
using UnityEngine;

namespace GlucoseWar.Core
{
    /// <summary>
    /// Generic prefab-bucketed object pool. Bullets / enemies / VFX are pooled
    /// to avoid runtime Instantiate/Destroy spikes (design rule #3).
    /// </summary>
    public class ObjectPool : Singleton<ObjectPool>
    {
        private readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();
        private readonly Dictionary<GameObject, GameObject> instanceToPrefab = new Dictionary<GameObject, GameObject>();
        private Transform root;

        protected override void Awake()
        {
            base.Awake();
            root = new GameObject("[Pooled]").transform;
            root.SetParent(transform);
        }

        public void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null) return;
            if (!pools.TryGetValue(prefab, out Queue<GameObject> q))
            {
                q = new Queue<GameObject>();
                pools[prefab] = q;
            }
            for (int i = 0; i < count; i++)
            {
                GameObject go = CreateNew(prefab);
                go.SetActive(false);
                q.Enqueue(go);
            }
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null) return null;
            if (!pools.TryGetValue(prefab, out Queue<GameObject> q))
            {
                q = new Queue<GameObject>();
                pools[prefab] = q;
            }

            GameObject go = q.Count > 0 ? q.Dequeue() : CreateNew(prefab);
            go.transform.SetPositionAndRotation(position, rotation);
            go.SetActive(true);

            foreach (var poolable in go.GetComponents<IPoolable>())
                poolable.OnSpawned();

            return go;
        }

        public void Despawn(GameObject go)
        {
            if (go == null) return;
            if (!instanceToPrefab.TryGetValue(go, out GameObject prefab))
            {
                go.SetActive(false);
                return;
            }

            foreach (var poolable in go.GetComponents<IPoolable>())
                poolable.OnDespawned();

            go.SetActive(false);
            go.transform.SetParent(root);
            pools[prefab].Enqueue(go);
        }

        private GameObject CreateNew(GameObject prefab)
        {
            GameObject go = Instantiate(prefab, root);
            instanceToPrefab[go] = prefab;
            return go;
        }
    }

    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
    }
}
