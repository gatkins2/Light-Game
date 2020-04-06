using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    GameObject[] roomList;                                   // List of all the rooms
    public bool travelling { get; private set; }             // If the camera is travelling to a new room
    Vector3 cameraPosition;                                  // Where the camera should be pointing

    // Start is called before the first frame update
    void Start()
    {
        roomList = GameObject.FindGameObjectsWithTag("RoomBounds");
    }

    // Update called every frame
    void Update()
    {
        // Move camera to new room
        if (travelling)
        {
            if ((transform.position - cameraPosition).magnitude < 0.1)
            {
                travelling = false;
                transform.position = cameraPosition;
            }
            else
                transform.position = Vector3.Lerp(transform.position, cameraPosition, 0.1f);
        }
    }

    // Move camera to a new room
    public void ChangeRoom(Vector2 point)
    {
        for (int i = 0; i < roomList.Length; i++)
            if (roomList[i].GetComponent<BoxCollider2D>().OverlapPoint(point))
            {
                cameraPosition = roomList[i].transform.position;
                cameraPosition.z = transform.position.z;
                travelling = true;
                break;
            }
    }

    // Check if a point is in bounds
    public bool PointInBounds(Vector2 point)
    {
        for (int i = 0; i < roomList.Length; i++)
            if (roomList[i].GetComponent<BoxCollider2D>().OverlapPoint(point))
                return true;
        return false;
    }

    // Return the room the object is in
    public GameObject GetObjectRoom(GameObject ob)
    {
        GameObject room = null;
        for (int i = 0; i < roomList.Length; i++)
            if (roomList[i].GetComponent<BoxCollider2D>().OverlapPoint(ob.transform.position))
            {
                room = roomList[i];
                break;
            }

        return room;
    }
}
