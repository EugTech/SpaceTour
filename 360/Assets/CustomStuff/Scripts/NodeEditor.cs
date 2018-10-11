
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class NodeEditor : EditorWindow
{
#if UNITY_EDITOR
    private List<RoomNode> windows = new List<RoomNode>();

    private Vector2 mousePos;

    private BaseRoomNode selectedNode;

    private bool Transitioning = false;

    private string roomMapName = null;
    private RoomMap SelectedRoomMap = null;

    [MenuItem("Window/Node Editor")]
    static void ShowEditor()
    {
        NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
    }

    private void OnGUI()
    {
        Event e = Event.current;
        mousePos = e.mousePosition;

        if (GUILayout.Button("Load"))
        {
            if (SelectedRoomMap != null)
            {
                windows = SelectedRoomMap.rooms;
                roomMapName = SelectedRoomMap.name;
            }
        }

        roomMapName = EditorGUILayout.TextField("roomMapName = ", roomMapName);
        SelectedRoomMap = (RoomMap)EditorGUILayout.ObjectField(SelectedRoomMap, typeof(RoomMap));

        if (GUILayout.Button("Save"))
        {
            // complex i know :/

            if (SelectedRoomMap == null)
            {
                RoomMap RM = new RoomMap();
                if (roomMapName != "")
                {
                    RM = CreateAsset<RoomMap>(roomMapName);
                }
                else
                {
                    RM = CreateAsset<RoomMap>();
                }

                Selection.activeObject = RM;


                for (int i = 0; i < windows.Count; i++)
                {

                    RoomNode rn = CreateAsset<RoomNode>(windows[i], windows[i].windowTitle, roomMapName);
                    RM.rooms.Add(rn);

                }
                SelectedRoomMap = RM;
                //RM.rooms = windows;
            }
            else if (!AssetDatabase.Contains(SelectedRoomMap))
            {
                RoomMap RM = new RoomMap();
                if (roomMapName != "")
                {
                    RM = CreateAsset<RoomMap>(roomMapName);
                }
                else
                {
                    RM = CreateAsset<RoomMap>();
                }

                Selection.activeObject = RM;


                for (int i = 0; i < windows.Count; i++)
                {
                    if (AssetDatabase.Contains(windows[i]))
                    {
                        Debug.Log(windows[i].name + "Is an asset already");
                        AssetDatabase.SaveAssets();
                        RM.rooms.Add(windows[i]);
                    }
                    else
                    {
                        RoomNode rn = CreateAsset<RoomNode>(windows[i], windows[i].windowTitle, roomMapName);
                        RM.rooms.Add(rn);
                    }
                }
                SelectedRoomMap = RM;
                //RM.rooms = windows;
            }
            else if (AssetDatabase.Contains(SelectedRoomMap))
            {
                Selection.activeObject = SelectedRoomMap;
                for (int i = 0; i < windows.Count; i++)
                {
                    if (AssetDatabase.Contains(windows[i]))
                    {
                       
                    }
                    else
                    {
                        RoomNode rn = CreateAsset<RoomNode>(windows[i], windows[i].windowTitle, roomMapName);
                        SelectedRoomMap.rooms.Add(rn);
                    }
                }
                AssetDatabase.SaveAssets();
            }

        }

        if (e.button == 1 && !Transitioning)
        {
            if (e.type == EventType.MouseDown)
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].WindowRect.Contains(mousePos))
                    {
                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (!clickedOnWindow)
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Add Room"), false, ContextCallback, "roomNode");

                    menu.ShowAsContext();
                    e.Use();
                }
                else
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, "makeTransition");
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Delete Room"), false, ContextCallback, "deleteNode");

                    menu.ShowAsContext();
                    e.Use();
                }
            }
        }
        else if (e.button == 0 && e.type == EventType.MouseDown && Transitioning)
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].WindowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }

            if (clickedOnWindow && !windows[selectIndex].Equals(selectedNode))
            {
                windows[selectIndex].SetInput(selectedNode, mousePos);
                Transitioning = false;

                selectedNode = null;
            }

            if (!clickedOnWindow)
            {
                Transitioning = false;
                selectedNode = null;
            }

            e.Use();
        }
        else if (e.button == 0 && e.type == EventType.MouseDown && !Transitioning)
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].WindowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }

            if (clickedOnWindow)
            {
                BaseRoomNode nodeToChange = (BaseRoomNode)windows[selectIndex].ClickedOnInput(mousePos);
                if (nodeToChange != null)
                {
                    nodeToChange.RmChild(windows[selectIndex]);
                    selectedNode = nodeToChange;
                    Transitioning = true;
                }
            }
        }

        if (Transitioning && selectedNode != null)
        {
            Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10, 10);

            DrawNodeCurve(selectedNode.WindowRect, mouseRect);

            Repaint();
        }

        try
        {
            foreach (RoomNode n in windows)
            {
                n.DrawCurves();
            }
        }
        catch (System.Exception A)
        {
            Debug.LogError(A);
        }

        BeginWindows();
        for (int i = 0; i < windows.Count; i++)
        {
            windows[i].WindowRect = GUILayout.Window(i, windows[i].WindowRect, DrawNodeWindow, windows[i].windowTitle);
        }
        EndWindows();
    }

    void DrawNodeWindow(int id)
    {
        windows[id].DrawWindow();
        GUI.DragWindow();
    }

    void ContextCallback(object obj)
    {
        string clb = obj.ToString();

        if (clb.Equals("roomNode"))
        {
            RoomNode rmNode = new RoomNode();
            rmNode.WindowRect = new Rect(mousePos.x, mousePos.y, 150, 200);

            windows.Add(rmNode);
        }
        else if (clb.Equals("makeTransition"))
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].WindowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }

            if (clickedOnWindow)
            {
                selectedNode = windows[selectIndex];
                Transitioning = true;
            }
        }
        else if (clb.Equals("deleteNode"))
        {
            bool clickedOnWindow = false;
            int selectIndex = -1;

            for (int i = 0; i < windows.Count; i++)
            {
                if (windows[i].WindowRect.Contains(mousePos))
                {
                    selectIndex = i;
                    clickedOnWindow = true;
                    break;
                }
            }

            if (clickedOnWindow)
            {
                BaseNode selectedNode = windows[selectIndex];
                windows.RemoveAt(selectIndex);

                foreach (BaseNode n in windows)
                {
                    n.NodeDeleted(selectedNode);
                }
            }
        }
    }

    public static void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x + end.width / 2, end.y + end.height / 2, 0);
        Vector3 startTangent = startPos + Vector3.right * 50;
        Vector3 endTangent = endPos + Vector3.left * 50;
        Color shadowColor = new Color(0, 1, 1, 0.06f);

        for (int i = 0; i < 3; i++)
        {
            Handles.DrawBezier(startPos, endPos, startTangent, endTangent, shadowColor, null, (i + 1) * 5);
        }

        Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Color.cyan, null, 1);
    }
    public T CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets/";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = path + "/New " + typeof(T).ToString() + ".asset";

        AssetDatabase.DeleteAsset(assetPathAndName);
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        //Selection.activeObject = asset;

        return asset;
    }
    public T CreateAsset<T>(string AName) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets/";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        //string assetPathAndName = path + "/" + AName + ".asset";
        string assetPathAndName = path + AName + ".asset";

        AssetDatabase.DeleteAsset(assetPathAndName);
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        //Selection.activeObject = asset;

        return asset;
    }
    public T CreateAsset<T>(T copy, string AName) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        asset = copy;

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets/";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = path + AName + ".asset";

        AssetDatabase.DeleteAsset(assetPathAndName);
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        //Selection.activeObject = asset;

        return asset;
    }
    public T CreateAsset<T>(T copy, string AName, string ParentName) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        asset = copy;

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {


            path = "Assets/";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        if (!AssetDatabase.IsValidFolder(path + ParentName))
        {
            Debug.Log(path + ParentName + " was not a valid folder");
            AssetDatabase.CreateFolder(path.Substring(0, path.Length - 1), ParentName);
        }
        if (AssetDatabase.IsValidFolder(path + ParentName))
        {
            string assetPathAndName = path + ParentName + "/" + AName + ".asset";

            AssetDatabase.DeleteAsset(assetPathAndName);
            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            //Selection.activeObject = asset;

            return asset;
        }
        else return null;
    }
#endif
}
#endif
