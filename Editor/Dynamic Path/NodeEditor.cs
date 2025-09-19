using UnityEngine;
using UnityEditor;
using BlueMuffinGames.Tools.DynamicPath;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Node node = (Node)target;

        if (GUILayout.Button("Recalculate Tangent"))
        {
            node.RecalculateTangent();
        }

        if (GUILayout.Button("Redraw"))
        {
            node.DrawDebugLines();
        }

        GUILayout.Label($"Slope: {node.M}\nTangent: {node.Tangent}");
    }
}