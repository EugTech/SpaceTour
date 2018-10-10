using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMappingScript : MonoBehaviour {

    public RoomMap Map = null;

    public GameObject Prefab = null;

    public bool Reload = true;

    List<GameObject> SpawnedObjects = new List<GameObject>();
	
	
	void Update () {
		
        if(Map != null && Prefab != null && Reload )
        {
            Reload = false;

            foreach(GameObject go in SpawnedObjects)
            {
                Destroy(go);
            }
            SpawnedObjects.Clear();

            foreach(RoomNode RN in Map.rooms)
            {
                if (RN != null)
                {
                    GameObject spawn = null;
                    if (RN.DirectAngle != ""){
                        spawn = Instantiate(Prefab, Quaternion.Euler(0, 0, float.Parse(RN.DirectAngle)) * transform.forward
                            , Quaternion.Euler(0, 0, float.Parse(RN.DirectAngle)), transform);
                    }
                    else
                    {
                        spawn = Instantiate(Prefab, Random.onUnitSphere
                            , transform.rotation, transform);
                    }

                    if (RN.RoomTexture != null)
                        spawn.GetComponent<Renderer>().material.mainTexture = RN.RoomTexture;

                    SpawnedObjects.Add(spawn);
                }
            }
        }

	}
}
