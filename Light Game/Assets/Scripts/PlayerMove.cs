﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    Pointer pointer;

	// Use this for initialization
	void Start ()
    {
        pointer = transform.GetChild(0).transform.GetChild(0).GetComponent<Pointer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Teleport on mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Teleport();
        }
	}

    // Called on collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    
    // Teleports the player to where the pointer is pointing
    void Teleport()
    {
        // Change rooms
        if (pointer.FinalObject != null && pointer.FinalObject.tag == "RoomChangeBox")
        {
            pointer.FinalObject.GetComponent<RoomChanger>().ChangeScene();
        }

        // Restart room if black hole hit
        else if (pointer.FinalObject != null && pointer.FinalObject.tag == "BlackHole")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);


        else if (pointer.FinalObject.tag == "AttachableSurface" && pointer.TeleportPoint != (Vector2)transform.position)
        {
            // Attempt teleport
            Vector2 oldPosition = transform.position;
            transform.position = pointer.TeleportPoint;

            // Revert position if collision is bad

            // Rotate to face up from object's normal
            transform.up = pointer.ObjectNormal;

        }

        else
            pointer.FlashRed();
    }
}
