using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavButton : MonoBehaviour {

        private Vector3 OGSize;
        private Renderer myRenderer;

        public Material inactiveMaterial;
        public Material gazedAtMaterial;

    public MapManager ParentManager;

    public float MaxScale = 2;
    public float MaxAngle = 45;

    public int RoomID = -1;

    bool Gazed = false;

        void Start()
        {
            OGSize = transform.localScale;
            myRenderer = GetComponent<Renderer>();
            SetGazedAt(false);
        }

        public void SetGazedAt(bool gazedAt)
        {
        Gazed = gazedAt;
            if (inactiveMaterial != null && gazedAtMaterial != null)
            {
                myRenderer.material = gazedAt ? gazedAtMaterial : inactiveMaterial;
                return;
            }
        }

    public void SetSizeOnView(Vector3 ViewDir)
    {
        float Angle = Vector3.Angle(transform.position, ViewDir);
        Debug.Log(Angle.ToString());
        if(Angle < MaxAngle)
        {
            transform.localScale = OGSize * (1f + (MaxScale - 1f) * ((MaxAngle - Angle) / MaxAngle));
        }
        else
        {
            transform.localScale = OGSize;
        }
        
    }

    public void HandleClicked()
    {
        if(ParentManager != null)
        {
            ParentManager.SwitchRooms(RoomID);
        }
        Debug.Log("Clicked " + name + " RoomID:" + RoomID.ToString());
    }

 }
