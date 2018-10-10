
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseNode : ScriptableObject {

    public Rect WindowRect;

    public bool hasInputs = false;

    public string windowTitle = "";

#if UNITY_EDITOR
    public virtual void DrawWindow()
    {
        windowTitle = EditorGUILayout.TextField("Title", windowTitle);
    }

    public abstract void DrawCurves();

    public virtual void SetInput(BaseNode input, Vector2 clickPos)
    {

    }

    public virtual void NodeDeleted(BaseNode node)
    {

    }

    public virtual BaseNode ClickedOnInput(Vector2 pos)
    {
        return null;
    }

    public virtual void AddChild(RoomNode baseRoomNode)
    {
        
    }
    public virtual void RmChild(RoomNode baseRoomNode)
    {
       
    }
#endif

}

