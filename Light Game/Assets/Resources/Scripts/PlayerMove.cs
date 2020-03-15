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

    Sprite defaultSprite;

    public bool Enabled { private get; set; }   // Whether player sprite and pointer are active
    public Pointer pointer { private get; set; }

	// Use this for initialization
	void Start ()
    {
        pointer = transform.GetChild(0).transform.GetChild(0).GetComponent<Pointer>();
        defaultSprite = GetComponent<SpriteRenderer>().sprite;
        Enabled = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Enabled)
        {
            // Reset pointer
            if (pointer == null)
                pointer = transform.GetChild(0).transform.GetChild(0).GetComponent<Pointer>();
            // Set sprite to active
            GetComponent<SpriteRenderer>().enabled = true;
            if (!(pointer is ColorPointer) || 
                Camera.main.GetComponent<PlayerSelector>().activePlayer == PlayerSelector.ColorPlayer.ALL ||
                Camera.main.GetComponent<PlayerSelector>().playerPointers[(int)Camera.main.GetComponent<PlayerSelector>().activePlayer] == pointer)
                pointer.Active = true;

            // Teleport on mouse click
            if (Input.GetMouseButtonDown(0))
            {
                Teleport();
            }
        }
        else
        {
            // Disable sprite and pointer
            GetComponent<SpriteRenderer>().enabled = false;
            pointer.Active = false;
        }
    }

    // Called on collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    
    // Teleports the player to where the pointer is pointing
    void Teleport()
    {
        if (pointer.Active)
        {
            // Change rooms
            if (pointer.FinalObject != null && pointer.FinalObject.tag == "RoomChangeBox")
            {
                pointer.FinalObject.GetComponent<RoomChanger>().ChangeScene();
            }

            // Restart room if black hole hit
            else if (pointer.FinalObject != null && pointer.FinalObject.tag == "BlackHole")
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            // Move to an attachable surface
            else if (pointer.FinalObject != null && pointer.FinalObject.tag == "AttachableSurface" && pointer.TeleportPoint != (Vector2)transform.position)
            {
                // Attempt teleport
                Vector2 oldPosition = transform.position;
                transform.position = pointer.TeleportPoint;

                // Revert position if collision is bad

                // Rotate to face up from object's normal
                transform.up = pointer.ObjectNormal;

                // Set light to trail after
                List<Vector2> lightList = pointer.path;
                GameObject light = GameObject.Instantiate(trailingLight, lightList[0], Quaternion.identity);
                light.GetComponent<TrailingLight>().player = this;
                light.GetComponent<TrailingLight>().path = lightList;

                // Set player to travelling
                Enabled = false;

                // Play teleport sound
                GetComponent<AudioSource>().Play();
            }

            // Refract through a prism
            else if (pointer.FinalObject != null && pointer.FinalObject.tag == "Prism")
            {
                // Attempt combine refract
                if (pointer is ColorPointer)
                {
                    if (!pointer.FinalObject.GetComponent<Prism>().CombineRefract())
                        pointer.ErrorFlash();
                    else
                    {
                        // Destroy the colored player object
                        Destroy(gameObject);
                        Camera.main.GetComponent<PlayerSelector>().PlayerRefracted = false;
                    }
                }

                // Attempt split refract
                else
                {
                    if (!pointer.FinalObject.GetComponent<Prism>().SplitRefract())
                        pointer.ErrorFlash();
                    else
                    {
                        // Destroy main player object
                        Destroy(gameObject);
                        Camera.main.GetComponent<PlayerSelector>().PlayerRefracted = true;
                    }
                }
            }

            else
                pointer.ErrorFlash();
        }
    }
}
