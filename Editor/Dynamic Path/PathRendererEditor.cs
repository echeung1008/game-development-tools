using UnityEngine;
using UnityEditor;
using BlueMuffinGames.Tools.DynamicPath;

[CustomEditor(typeof(PathRenderer))]
public class PathRendererEditor : Editor
{
    Vector2 scrollPosition = Vector2.zero;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PathRenderer pathRenderer = (PathRenderer)target;

        if (GUILayout.Button("Render"))
        {
            pathRenderer.RenderPath(out pathRenderer.renderResult);
        }

        if (!string.IsNullOrEmpty(pathRenderer.renderResult))
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label(pathRenderer.renderResult);

            GUILayout.EndScrollView();
        }
    }
}