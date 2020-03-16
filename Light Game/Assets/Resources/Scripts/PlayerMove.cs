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
                Camera.main.GetComponent<PlayerSelector>().activePlayer == PlayerColor.ALL ||
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
            // Restart room if black hole hit
            if (pointer.FinalObject != null && pointer.FinalObject.tag == "BlackHole")
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            // Move to an attachable surface
            else if (pointer.FinalObject != null && pointer.FinalObject.tag == "AttachableSurface" && pointer.TeleportPoint != (Vector2)transform.position)
            {
                // Check if point is in a room
                if (!Camera.main.GetComponent<RoomController>().PointInBounds(pointer.TeleportPoint))
                    pointer.ErrorFlash();

                // Attempt teleport
                else
                {
                    Vector2 oldPosition = transform.position;
                    transform.position = pointer.TeleportPoint;

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
                    if (Camera.main.GetComponent<PlayerSelector>().PlayerRefracted)
                    {
                        bool audioPlaying = false;
                        PlayerMove[] audioList = FindObjectsOfType<PlayerMove>();
                        foreach (PlayerMove source in audioList)
                            if (source.GetComponent<AudioSource>().isPlaying)
                                audioPlaying = true;
                        if (!audioPlaying)
                            GetComponent<AudioSource>().Play();
                    }
                    else
                        GetComponent<AudioSource>().Play();
                }
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

            // Hit a menu button
            else if (pointer.FinalObject != null && pointer.FinalObject.tag == "MenuButton")
                pointer.FinalObject.GetComponent<CustomMenuButton>().ButtonAction();

            else
                pointer.ErrorFlash();
        }
    }
}
