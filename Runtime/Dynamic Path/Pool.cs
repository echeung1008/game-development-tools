using UnityEngine;
using UnityEngine.Pool;

namespace BlueMuffinGames.Tools.DynamicPath
{
    public abstract class Pool<T1> : MonoBehaviour where T1 : Component, IPoolable
    {
        [SerializeField]
        private GameObject objectPrefab;

        [SerializeField]
        private int poolSize = 100;

        [SerializeField]
        private int maxSize = 500;

        public static ObjectPool<GameObject> pool { get; private set; }

        private void Awake() => Initialize();
        private void OnValidate() => Initialize();

        private (GameObject, int, int) lastInitialize;

        protected virtual void Initialize()
        {
            if (lastInitialize == (objectPrefab, poolSize, maxSize)) return;

            if (pool != null) pool.Clear();

            pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(objectPrefab, transform),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: poolSize,
                maxSize: maxSize
            );

            lastInitialize = (objectPrefab, poolSize, maxSize);
        }

        protected virtual void OnDestroy()
        {
            pool?.Clear();
        }

        public T1 GetObject()
        {
            if (pool != null) return pool.Get().GetComponent<T1>();
            else return Instantiate(objectPrefab, transform).GetComponent<T1>();
        }
        public static void ReleaseObject(params T1[] objects) { foreach (T1 o in objects) { o.Release(); pool.Release(o.gameObject); } }
        public static void ReleaseObjects(T1[] objects) { foreach (T1 o in objects) { o.Release(); pool.Release(o.gameObject); } }
    }
}
