using UnityEngine;
using UnityEditor;
using BlueMuffinGames.Tools.DynamicPath;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private string _insertIndex = "0";
    private Vector2 _scrollPosition;

    private string NodeCount
    {
        get
        {
            return _nodeCount.ToString();
        }
        set
        {
            if(int.TryParse(value, out int newCount) && newCount != _nodeCount){
                Path path = (Path)target;
                int difference = newCount - path.nodes.Count;
                if(difference > 0)
                {
                    for(int i = 0; i < difference; i++)
                    {
                        path.AddNodeToEnd();
                    }
                }
                else if (difference < 0)
                {
                    for (int i = 0; i < -difference; i++)
                    {
                        path.DeleteNode(path.nodes.Count - 1);
                    }
                }
                _nodeCount = newCount;
            }
        }
    }

    private int _nodeCount = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(5f);

        DisplayAddNode(out bool pressedFront, out bool pressedEnd, out bool pressedInsert);

        GUILayout.Space(5f);

        DisplayNodeList(out int moveUpIndex, out int moveDownIndex, out int deleteIndex);

        Path path = (Path)target;
        if (pressedFront) path.AddNodeToFront();
        if (pressedEnd) path.AddNodeToEnd();
        if (pressedInsert && int.TryParse(_insertIndex, out int index)) path.InsertNewNode(index);
        if (moveUpIndex >= 0) path.MoveNodeUp(moveUpIndex); 
        if (moveDownIndex >= 0) path.MoveNodeDown(moveDownIndex);
        if (deleteIndex >= 0) path.DeleteNode(deleteIndex);

        _nodeCount = path.nodes.Count;
    }

    void OnSceneGUI()
    {
        var path = (Path)target;
        if (path == null || path.nodes == null) return;

        // Hide the default Transform gizmo while editing to avoid confusion.
        bool prevHidden = Tools.hidden;
        Tools.hidden = true;

        try
        {
            for (int i = 0; i < path.nodes.Count; i++)
            {
                var node = path.nodes[i];
                if (!node) continue;

                var handleRot = Tools.pivotRotation == PivotRotation.Local
                    ? node.transform.rotation
                    : Quaternion.identity;

                EditorGUI.BeginChangeCheck();

                Vector3 newPos = Handles.PositionHandle(node.transform.position, handleRot);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(node.transform, "Move Path Node");
                    node.transform.position = newPos;
                    EditorUtility.SetDirty(node.transform);
                }

                // Optional label
                Handles.Label(node.transform.position + Vector3.up * 0.2f, $"Node {i}");
            }
        }
        finally
        {
            Tools.hidden = prevHidden; // restore
        }
    }

    private void DisplayAddNode(out bool pressedFront, out bool pressedEnd, out bool pressedInsert)
    {
        GUILayout.Label("Add Node");

        GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

                pressedFront = GUILayout.Button("Add Front");
                pressedEnd = GUILayout.Button("Add End");

            GUILayout.EndHorizontal();

            DisplayInsertNode(out pressedInsert);

        GUILayout.EndVertical();
    }

    private void DisplayInsertNode(out bool pressed)
    {
        GUILayout.BeginHorizontal();

            GUILayout.Label("Insert Node");
            _insertIndex = GUILayout.TextField(_insertIndex);
            pressed = GUILayout.Button("Insert");

        GUILayout.EndHorizontal();
    }

    private void DisplayNodeList(out int moveUpIndex, out int moveDownIndex, out int deleteIndex)
    {
        moveUpIndex = -1;
        moveDownIndex = -1;
        deleteIndex = -1;

        Path path = (Path)target;

        GUILayout.BeginHorizontal();

            GUILayout.Label("Nodes");
            NodeCount = GUILayout.TextField(NodeCount);

        GUILayout.EndHorizontal();

        if (path.nodes == null) return;

        GUILayout.Space(2f);

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        for(int i = 0; i < path.nodes.Count; i++)
        {
            GUILayout.BeginHorizontal();

                GUILayout.Label("Node " + i);
                if (GUILayout.Button("Delete")) deleteIndex = i;

                GUILayout.BeginVertical();

                    if (GUILayout.Button("↑")) moveUpIndex = i;
                    if (GUILayout.Button("↓")) moveDownIndex = i;

                GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();

            GUILayout.Space(2f);
        }

        GUILayout.EndScrollView();
    }
}