using UnityEngine;
using UnityEditor;
using BlueMuffinGames.Tools.DynamicPath;

[CustomEditor(typeof(RemeshedPath))]
public class RemeshedPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Remesh"))
        {
            RemeshedPath remeshedPath = (RemeshedPath)target;
            switch (remeshedPath.remeshMode)
            {
                case RemeshMode.Simple:
                    remeshedPath.SimpleRemesh(true);
                    break;
                case RemeshMode.Complex:
                    remeshedPath.ComplexRemesh(true);
                    break;
            }
        }

        if (GUILayout.Button("Clear"))
        {
            RemeshedPath remeshedPath = (RemeshedPath)target;
            remeshedPath.Clear(true);
        }
    }
}