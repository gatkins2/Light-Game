using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    GameObject trailingLight;
    [SerializeField]
    Sprite emptySprite;

    Pointer pointer;
    Sprite defaultSprite;

    public bool Travelling { private get; set; }

	// Use this for initialization
	void Start ()
    {
        pointer = transform.GetChild(0).transform.GetChild(0).GetComponent<Pointer>();
        defaultSprite = GetComponent<SpriteRenderer>().sprite;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Teleport on mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Teleport();
        }

        // Set sprite to inactive while light sprite travels
        if (!Travelling)
            GetComponent<Animator>().SetBool("Travelling", false);
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

            // Set light to trail after
            List<Vector2> lightList = new List<Vector2>();
            foreach (Vector2 v in pointer.path)
                lightList.Add(v);
            GameObject light = GameObject.Instantiate(trailingLight, lightList[0], Quaternion.identity);
            light.GetComponent<TrailingLight>().player = this;
            light.GetComponent<TrailingLight>().path = lightList;
            Travelling = true;
            GetComponent<Animator>().SetBool("Travelling", true);

            // Play teleport sound
            GetComponent<AudioSource>().Play();
        }

        else
            pointer.FlashRed();
    }
}
