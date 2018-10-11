using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public RoomMap Map;

    public float Size = 50;

    public int CurrentRoom = -1;

    Renderer rend;
    public Renderer ChildRend;

    public GameObject ButtonPrefab;

    private List<GameObject> SpawnedRoomButtons = new List<GameObject>();

    private bool Switching = false;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        //Size = (transform.localScale.x /2f) * 0.7f ;

        if (Map != null)
        {
            CurrentRoom = 0;
        }

        StartCoroutine(ISpawnRoom());
        Debug.Log("Start");
    }

    public Vector3 GetRoomDirectionalPosition(int childId)
    {
        if(Map != null)
        {
            if(Map.rooms.Count > CurrentRoom && CurrentRoom >= 0)
            {
                if(Map.rooms[CurrentRoom].ChildRooms.Count > childId && childId >= 0)
                {
                    Vector3 myLoc = Map.rooms[CurrentRoom].RoomLocation;
                    Vector3 DestinationRoom = Map.rooms[CurrentRoom].ChildRooms[childId].RoomLocation;
                    Vector3 ans = DestinationRoom - myLoc;

                    if(ans.magnitude <= 0)
                    {
                        ans = Vector3.forward * Size;
                    }
                    else
                    {
                        ans = ans.normalized * Size;
                    }

                    return ans;
                }
                {
                    Debug.LogError("Child ID is out of bounds");
                    return Vector3.zero;
                }
            }
            else
            {
                Debug.LogError("CurrentRoom is out of bounds");
                return Vector3.zero;
            }
        }
        else
        {
            Debug.LogError("Map OBJ is null");
            return Vector3.zero;
        }
    }

    public Quaternion GetCurrentRoomRotation()
    {
        if(Map != null)
        {
            if (Map.rooms.Count > CurrentRoom && CurrentRoom >= 0)
            {
                int parsed = 0;
                try
                {
                    parsed = int.Parse(Map.rooms[CurrentRoom].DirectAngle);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
                Quaternion ans = Quaternion.Euler(0f, parsed ,0f);
                return ans;
            }
            else
            {
                return Quaternion.identity;
            }
        }
        else
        {
            return Quaternion.identity;
        }
    }

    void spawnButton(int id)
    {
        Vector3 spawnLoc = GetRoomDirectionalPosition(id);
        if(spawnLoc == Vector3.zero)
        {
            return;
        }
        GameObject Spawn = Instantiate(ButtonPrefab, spawnLoc, Quaternion.identity, null);
        Spawn.transform.rotation = Quaternion.LookRotation(-transform.position + Spawn.transform.position, Vector3.up);
        NavButton SpawnedNav = Spawn.GetComponent<NavButton>();
        SpawnedNav.RoomID = id;
        SpawnedNav.ParentManager = this;
    }

    public void SwitchRooms(int DirtyID)
    {
        if(!Switching)
        {
            StartCoroutine(ISwitchRooms(DirtyID));
        }
    }

    IEnumerator ISwitchRooms(int DirtyID)
    {
        Switching = true;
        if (Map != null)
        {
            if (Map.rooms.Count > CurrentRoom && CurrentRoom >= 0)
            {
                if (Map.rooms[CurrentRoom].ChildRooms.Count > DirtyID && DirtyID  >= 0)
                {
                    // delete all buttons
                    foreach(GameObject go in SpawnedRoomButtons)
                    {
                        Destroy(go);
                    }
                    SpawnedRoomButtons.Clear();
                    //fade in blocker
                    for (int i = 0; i <= 30; i++)
                    {
                        rend.material.SetFloat("_Transparency", 0.5f - (float)i/60f);
                        yield return new WaitForSeconds(0);
                    }
                    rend.material.SetFloat("_Transparency", 0f);
                    // change current room

                    CurrentRoom = Map.rooms.IndexOf(Map.rooms[CurrentRoom].ChildRooms[DirtyID]);

                    // switch texture

                    rend.material.mainTexture = Map.rooms[CurrentRoom].RoomTexture;

                    // rotate sphere to oreintation
                    try
                    {
                        transform.rotation = Quaternion.Euler(0, int.Parse(Map.rooms[CurrentRoom].DirectAngle), 0);
                    }
                    catch(System.Exception e)
                    {
                        transform.rotation = Quaternion.identity;
                        Debug.LogError(e);
                    }

                    // spawn new buttons

                    for(int i = 0; i < Map.rooms[CurrentRoom].ChildRooms.Count; i++)
                    {
                        spawnButton(i);
                    }

                    //fade out blocker
                    for (int i = 0; i <= 30; i++)
                    {
                        rend.material.SetFloat("_Transparency", (float)i / 60f);
                        yield return new WaitForSeconds(0);
                    }
                    rend.material.SetFloat("_Transparency", 0.5f);
                }
            }
        }
                    yield return new WaitForSeconds(0);
        Switching = false;
    }

    IEnumerator ISpawnRoom()
    {
        Switching = true;
        if (Map != null)
        {
            if (Map.rooms.Count > CurrentRoom && CurrentRoom >= 0)
            {
                //Debug.Log(rend.material.HasProperty("_Transparency").ToString());
                rend.material.mainTexture = Map.rooms[CurrentRoom].RoomTexture;

                for (int i = 0; i <= 30; i++)
                {
                    rend.material.SetFloat("_Transparency", (float)i / 60f);
                    yield return new WaitForSeconds(0);
                }
                rend.material.SetFloat("_Transparency", 0.5f);
                
                    // spawn new buttons

                    for (int i = 0; i < Map.rooms[CurrentRoom].ChildRooms.Count; i++)
                    {
                        spawnButton(i);
                    }

                    
                
            }
        }
        yield return new WaitForSeconds(0);
        Switching = false;
    }
}
