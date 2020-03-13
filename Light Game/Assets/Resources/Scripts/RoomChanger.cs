using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomChanger : MonoBehaviour
{
    public void ChangeScene()
    {
        GameObject roomBounds = GameObject.FindGameObjectsWithTag("RoomBounds")[0];
        BoxCollider2D boundingBox = roomBounds.GetComponent<BoxCollider2D>();
        float width = boundingBox.size.x;
        float height = boundingBox.size.y;
        int roomIndex = SceneManager.GetActiveScene().buildIndex;


        // Go up 1 room
        if (transform.position.y > roomBounds.transform.position.y + (height / 2))
            roomIndex += 5;

        // Go down 1 room
        else if (transform.position.y < roomBounds.transform.position.y - (height / 2))
            roomIndex -= 5;

        // Go right 1 room
        else if (transform.position.x > roomBounds.transform.position.x + (width / 2))
            roomIndex += 1;

        // Go left 1 room
        else if (transform.position.x < roomBounds.transform.position.x - (width / 2))
            roomIndex -= 1;

        // Load scene
        SceneManager.LoadScene(roomIndex);
    }
}
