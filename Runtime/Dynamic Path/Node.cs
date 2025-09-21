using UnityEngine;

namespace BlueMuffinGames.Tools.DynamicPath
{
    [System.Serializable]
    public class Node : MonoBehaviour, IPoolable
    {
        [SerializeField] private Node _prev;
        [SerializeField] private Node _next;

        public float X
        {
            get => transform.position.x;
            private set
            {
                var p = transform.position;
                p.x = value;
                transform.position = p;
                RecalculateTangent();
            }
        }

        public float Y
        {
            get => transform.position.y;
            private set
            {
                var p = transform.position;
                p.y = value;
                transform.position = p;
                RecalculateTangent();
            }
        }

        public float Z
        {
            get => transform.position.z;
            private set
            {
                var p = transform.position;
                p.z = value;
                transform.position = p;
                RecalculateTangent();
            }
        }

        public float M { get; private set; }

        public float W { get; set; }

        public Vector3 Tangent
        {
            get
            {
                if (float.IsInfinity(M))
                    return Vector3.up;
                else
                    return new Vector3(1f, M, 0f).normalized;
            }
        }
        public Vector3 Normal
        {
            get
            {
                if (float.IsInfinity(M)) return Vector3.right;
                else if (M == 0f) return Vector3.up;
                Vector3 result = new Vector3(-1, 1 / M, 0f).normalized;
                if (result.x < 0f && result.y < 0f) result *= -1f;
                return result;
            }
        }
        public bool ConcaveUp
        {
            get
            {
                if (_prev != null && _next != null) return _prev.M < M && M < _next.M;
                else return false;
            }
        }
        public bool ConcaveDown
        {
            get
            {
                if (_prev != null && _next != null) return _prev.M > M && M > _next.M;
                else return false;
            }
        }

        public Node Previous
        {
            get => _prev;
            set { _prev = value; RecalculateTangent(); }
        }

        public Node Next
        {
            get => _next;
            set { _next = value; RecalculateTangent(); }
        }

        public Vector3 Position => transform.position;
        public Vector3 LocalPosition => transform.localPosition;

        private Vector3 _lastPos;

        public void RecalculateTangent()
        {
            TangentComponent a = new TangentComponent(0, 0f);
            TangentComponent b = new TangentComponent(0, 0f);

            if (Previous != null && Next != null)
            {
                float distanceA = Distance(Previous, this);
                float distanceB = Distance(Next, this);
                float totalDistance = distanceA + distanceB;

                a = new TangentComponent(Slope(Previous, this), 1.0f - distanceA / totalDistance);
                b = new TangentComponent(Slope(this, Next), 1.0f - distanceB / totalDistance);
            }
            else if (Previous == null && Next != null)
            {
                b = new TangentComponent(Slope(this, Next), 1f);
            }
            else if (Next == null && Previous != null)
            {
                a = new TangentComponent(Slope(Previous, this), 1f);
            }

            M = a.WeightedValue + b.WeightedValue;
        }

        private void Awake() => SyncFromTransform(force: true);
        private void OnEnable() => SyncFromTransform(force: true);
        private void OnValidate() => SyncFromTransform(force: true);
        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) { SyncFromTransform(); return; }
#endif
            SyncFromTransform();
        }

        private void SyncFromTransform(bool force = false)
        {
            if (force || transform.position != _lastPos)
            {
                if (_prev != null) transform.position = new Vector3(Mathf.Max(_prev.X, transform.position.x), transform.position.y, transform.position.z);
                if (_next != null) transform.position = new Vector3(Mathf.Min(_next.X, transform.position.x), transform.position.y, transform.position.z);
                _lastPos = transform.position;
                RecalculateTangent();
                Previous?.RecalculateTangent();
                Next?.RecalculateTangent();
            }
        }

        private struct TangentComponent
        {
            public float slope;
            public float weight;
            public float WeightedValue => slope * weight;
            public TangentComponent(float slope, float weight) { this.slope = slope; this.weight = weight; }
        }

        public static float Distance(Node a, Node b) => Vector2.Distance(new Vector2(a.X, a.Y), new Vector2(b.X, b.Y));

        public static float Slope(Node a, Node b) => Mathf.Approximately(b.X, a.X) ? float.PositiveInfinity : (b.Y - a.Y) / (b.X - a.X);

        public static Vector3 Intersection(Node a, Node b)
        {
            float x = (a.M * a.X + b.Y - a.Y - b.M * b.X) / (a.M - b.M);

            return new Vector3(
                x,
                a.M * (x - a.X) + a.Y,
                0f
            );
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(Position, 0.2f);
            DrawDebugLines();
        }

        public void DrawDebugLines()
        {
            if (Next != null) Debug.DrawLine(Position, Next.Position, Color.red);
            DrawExtension(Tangent, 1f, Color.yellow);
            DrawExtension(Normal, 1f, Color.blue, true);
        }

        private void DrawExtension(Vector3 dir, float halfLength, Color color, bool positiveOnly = false)
        {
            Debug.DrawLine(Position, Position + halfLength * dir, color);
            //if(!positiveOnly) Debug.DrawLine(Position, Position - halfLength * dir, color);
        }

        public void ClearLinks() { _prev = null; _next = null; }

        public void Release()
        {
            ClearLinks();
        }
    }
}
