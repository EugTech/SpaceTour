
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNode : BaseRoomNode {

    public string RoomName = "Room";

    public string DirectAngle = "";

    public Vector3 RoomLocation = Vector3.zero;

    private dummyVar dVr;

    public Texture2D RoomTexture;

    public List<RoomNode> ParentRoomNodes = new List<RoomNode>();
    private List<Rect> ParentNodeRects = new List<Rect>();

    public List<RoomNode> ChildRooms = new List<RoomNode>();


    enum dummyVar
    {
        option1,
        option2,
        option3,
    }

#if UNITY_EDITOR

    public RoomNode()
    {
        windowTitle = "Room Node";

        hasInputs = true;
    }

    public override void DrawWindow()
    {
        base.DrawWindow();

        RoomName = EditorGUILayout.TextField("Room Name", RoomName);

        //dVr = (dummyVar)EditorGUILayout.EnumPopup("LOL : ", dVr);

        RoomLocation = EditorGUILayout.Vector3Field("Room Location", RoomLocation);

        DirectAngle = EditorGUILayout.TextField("Angle", DirectAngle);

        //if (GUILayout.Button("THingy"))
        //{
        //    DoTHingy();
        //}

        RoomTexture = (Texture2D)EditorGUILayout.ObjectField("Image", RoomTexture, typeof(Texture2D), false);

        Event e = Event.current;

        for (int i = 0; i < ParentRoomNodes.Count; i++)
        {

            string ParentNodeTitle = "None";
            if (ParentRoomNodes[i])
            {
                ParentNodeTitle = ParentRoomNodes[i].getResult();
            }

            GUILayout.Label("Parent : " + ParentNodeTitle);

            if (e.type == EventType.Repaint)
            {
                while (ParentRoomNodes.Count > ParentNodeRects.Count)
                {
                    ParentNodeRects.Add(new Rect());
                }
                while (ParentRoomNodes.Count < ParentNodeRects.Count)
                {
                    ParentNodeRects.RemoveAt(ParentNodeRects.Count - 1);
                }
                ParentNodeRects[i] = GUILayoutUtility.GetLastRect();
            }
        }
        if (GUILayout.Button("-"))
        {
            RmParent();
        }
        if (GUILayout.Button("+"))
        {
            // add a slot for another parent
            AddParent();
        }

        for (int i = 0; i < ChildRooms.Count; i++)
        {
            string ChildTitle = "None";
            if (ChildRooms[i])
            {
                ChildTitle = ChildRooms[i].getResult();
            }
            GUILayout.Label("child : " + ChildTitle);
        }
    }

    public override string getResult()
    {
        return RoomName;
    }

    public override void DrawCurves()
    {
        for (int i = 0; i < ParentRoomNodes.Count; i++)
        {
            if (ParentRoomNodes[i])
            {
                Rect rect = WindowRect;
                rect.x += ParentNodeRects[i].x;
                rect.y += ParentNodeRects[i].y + ParentNodeRects[i].height / 2;
                rect.width = 1;
                rect.height = 1;

                NodeEditor.DrawNodeCurve(ParentRoomNodes[i].WindowRect, rect);


            }
        }
    }

    public override void NodeDeleted(BaseNode node)
    {
        for (int i = 0; i < ParentRoomNodes.Count; i++)
        {
            if (node.Equals(ParentRoomNodes[i]))
            {
                ParentRoomNodes[i] = null;
            }
        }
        try
        {
            ChildRooms.RemoveAll((RoomNode n) => n.Equals((RoomNode)node));
            //RmChild((BaseRoomNode)node);
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    public override BaseNode ClickedOnInput(Vector2 pos)
    {
        BaseNode ans = null;

        pos.x -= WindowRect.x;
        pos.y -= WindowRect.y;
        for (int i = 0; i < ParentRoomNodes.Count; i++)
        {
            if (ParentNodeRects[i].Contains(pos))
            {
                ans = ParentRoomNodes[i];
                ParentRoomNodes[i] = null;
                break;
            }
        }

        return ans;
    }

    public override void SetInput(BaseNode input, Vector2 clickPos)
    {
        clickPos.x -= WindowRect.x;
        clickPos.y -= WindowRect.y;
        for (int i = 0; i < ParentRoomNodes.Count; i++)
        {
            if (ParentNodeRects[i].Contains(clickPos))
            {
                ParentRoomNodes[i] = (RoomNode)input;
                input.AddChild(this);
                break;
            }
        }
    }

    void DoTHingy()
    {

    }

    void AddParent()
    {
        ParentRoomNodes.Add(null);
    }
    void RmParent()
    {
        ParentRoomNodes.RemoveAt(ParentRoomNodes.Count - 1);
    }

    public override void AddChild(RoomNode baseRoomNode)
    {
        ChildRooms.Add(baseRoomNode);
    }
    public override void RmChild(RoomNode baseRoomNode)
    {
        ChildRooms.Remove(baseRoomNode);
    }
#endif
}

