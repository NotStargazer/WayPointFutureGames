using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Waypoints))]
public class WaypointsEditor : Editor
{
    Vector2 scrollPos;
    int selectedIndex;
    bool selected;

    public override void OnInspectorGUI()
    {
        Waypoints wp = (Waypoints)target;

        Undo.RecordObject(wp, "Waypoints");

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(100));

        if (wp.m_Waypoints.Count > 0)
        {
            for (int p = 0; p < wp.m_Waypoints.Count; p++)
            {
                GUILayout.BeginHorizontal();

                wp.m_Waypoints[p] = EditorGUILayout.Vector3Field("Point " + (p + 1), wp.m_Waypoints[p]);
                if (!EditorGUIUtility.editingTextField)
                {
                    EditorWindow view = EditorWindow.GetWindow<SceneView>();
                    view.Repaint();
                }

                if (GUILayout.Button("Remove", GUILayout.Width(55)))
                {
                    RemovePoint(p);
                }
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("Add Point"))
        {
            AddPoint();
        }
    }

    void RemovePoint(int index)
    {
        Waypoints wp = (Waypoints)target;

        wp.m_Waypoints.RemoveAt(index);
    }

    void AddPoint()
    {
        Waypoints wp = (Waypoints)target;

        Camera sceneCam = SceneView.GetAllSceneCameras()[0];
        wp.m_Waypoints.Add(sceneCam.transform.position + sceneCam.transform.forward * 2);
    }

    void AddPoint(int index, Vector3 pos)
    {
        Waypoints wp = (Waypoints)target;

        Camera sceneCam = SceneView.GetAllSceneCameras()[0];
        wp.m_Waypoints.Insert(index, pos);
    }

    public void OnSceneGUI()
    {
        Waypoints wp = (Waypoints)target;

        Undo.RecordObject(wp, "Waypoints");

        GUIStyle textStyle = new GUIStyle(GUIStyle.none);
        textStyle.fontSize = 50;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.alignment = TextAnchor.MiddleCenter;

        if (selected)
        {
            Event e = Event.current;
            GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
            for (int p = 0; p < wp.m_Waypoints.Count; p++)
            {
                Handles.color = p == selectedIndex ? Handles.selectedColor : new Color(1, 0.1f, 1, 0.5f);

                Handles.DrawWireCube(wp.m_Waypoints[p], Vector3.one / 4);
                Handles.Label(wp.m_Waypoints[p] + Vector3.up * 0.5f, (p + 1).ToString(), textStyle);
            }

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    AddPoint(selectedIndex + 1, hit.point);
                    selected = false;
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Keyboard);
                }
            }
        }
        else
        {
            if (wp.m_Waypoints.Count > 0)
            {
                for (int p = 0; p < wp.m_Waypoints.Count; p++)
                {
                    Handles.Label(wp.m_Waypoints[p] + Vector3.up * 0.5f, (p + 1).ToString(), textStyle);

                    if (Tools.current == Tool.Move)
                    {
                        wp.m_Waypoints[p] = Handles.PositionHandle(wp.m_Waypoints[p], Quaternion.identity);
                    }
                    else
                    {
                        Handles.color = new Color(1, 0.1f, 1, 0.5f);
                        if (Handles.Button(wp.m_Waypoints[p], Quaternion.identity, 0.25f, 0.25f, Handles.CubeHandleCap))
                        {
                            selected = true;
                            selectedIndex = p;
                        }

                    }
                }
            }
        }
        Handles.color = Color.white;

        Handles.DrawPolyLine(wp.m_Waypoints.ToArray());
        Handles.DrawLine(wp.m_Waypoints[0], wp.m_Waypoints[wp.m_Waypoints.Count - 1]);
    }
}
