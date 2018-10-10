using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RoomMap", menuName = "RoomMap")]
public class RoomMap : ScriptableObject {

    public List<RoomNode> rooms = new List<RoomNode>();

    public int StartingRoom = 0;
}
