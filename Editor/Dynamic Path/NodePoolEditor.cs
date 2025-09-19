using UnityEngine;
using UnityEditor;
using BlueMuffinGames.Tools.DynamicPath;

[CustomEditor(typeof(NodePool))]
public class NodePoolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(NodePool.pool != null)
        {
            GUILayout.Label("Active objects: " + NodePool.pool.CountActive);
        }

    }
}