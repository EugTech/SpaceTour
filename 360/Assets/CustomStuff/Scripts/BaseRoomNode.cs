
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseRoomNode : BaseNode {
#if UNITY_EDITOR
    public virtual string getResult()
    {
        return "None";
    }

    public override void DrawCurves()
    {
        throw new System.NotImplementedException();
    }
#endif
}

