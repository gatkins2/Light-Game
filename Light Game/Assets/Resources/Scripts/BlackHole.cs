using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public GameObject BlackHoleExit;                // Other black hole to travel to
    public int FrameBuffer { private get; set; }    // Frame buffer for setting activity
    public bool Active { get; set; }                // Whether the black hole is active
    public Pointer Pointer { get; private set; }    // The pointer attached to the black hole

    Transform sprite;                               // Black hole sprite to rotate

    // Start is called before the first frame update
    void Start()
    {
        // Get sprite and pointer
        sprite = transform.GetChild(0);
        Pointer = transform.GetChild(1).GetComponent<Pointer>();

        // Set pointer to inactive
        Pointer.Active = false;

        // Set default vars
        FrameBuffer = 0;
        Active = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate sprite
        sprite.RotateAround(transform.position, Vector3.forward, -18f * Time.deltaTime);

        // Set activity
        if (FrameBuffer > 0)
            FrameBuffer--;
        else
            Active = false;

        if (Active)
        {
            // Activate pointer
            Pointer.Active = true;
        }

        else
        {
            // Deactiavte pointer
            Pointer.Active = false;
        }
    }
}
