using UnityEngine;
using UnityEngine.Pool;

namespace BlueMuffinGames.Tools.DynamicPath
{
    public class NodePool : Pool<Node>
    {
        public static NodePool Instance { get; private set; } = null;

        public static Node GetNode() => Instance?.GetObject();
        public static void ReleaseNode(Node node) => ReleaseObject(node);
        public static void ReleaseNodes(Node[] nodes) => ReleaseObjects(nodes);

        protected override void Initialize()
        {
            base.Initialize();
            Instance = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }
    }
}
